using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServiceBinGroup : BaseService<BinGroup, BinGroupDto>, IServiceBinGroup
    {
        private readonly IRepositoryBin _binRepository;
        private readonly IRepositoryService _serviceRepository;

        public ServiceBinGroup(IRepositoryBinGroup repository, IRepositoryBin binRepository, IRepositoryService serviceRepository)
            : base(repository)
        {
            _binRepository = binRepository;
            _serviceRepository = serviceRepository;
        }

        public override IQueryable<BinGroup> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override BinGroupDto Converter(BinGroup entity)
        {
            var dto = new BinGroupDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Bins = entity.Bins != null ? entity.Bins.Select(b => new BinDto
                {
                    Name = b.Name,
                    Id = b.Id,
                    Active = b.Active,
                    BankDtoId = b.BankId,
                    CardType = (CardTypeDto)b.CardType,
                    GatewayId = b.GatewayId,
                    Value = b.Value,
                    Country = b.Country,
                    BankDto = b.Bank == null ? null : new BankDto
                    {
                        Code = b.Bank.Code,
                        Id = b.Bank.Id,
                        Name = b.Bank.Name,
                    }
                }).ToList() : null,
                Services = entity.Services != null ? entity.Services.Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                }).ToList() : null
            };

            return dto;

        }

        public override BinGroup Converter(BinGroupDto entity)
        {
            return new BinGroup
            {
                Id = entity.Id,
                Name = entity.Name,
                Bins = entity.Bins != null ? entity.Bins.Select(b => new Bin
                {
                    Name = b.Name,
                    Id = b.Id,
                    Active = b.Active,
                    BankId = b.BankDtoId,
                    CardType = (CardType)b.CardType,
                    GatewayId = b.GatewayId,
                    Value = b.Value,
                    Country = b.Country
                }).ToList() : null,
                Services = entity.Services != null ? entity.Services.Select(s => new Service
                {
                    Id = s.Id
                }).ToList() : null
            };
        }

        public IEnumerable<BinGroupDto> GetDataForTable(BinGroupFilterDto filters)
        {
            var query = Repository.AllNoTracking();
            //var query = Repository.AllNoTracking(null, x => x.Bins.Select(b => b.Bank));

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            if (filters.ServiceId.HasValue)
                query = query.Where(sc => sc.Services.Any(s => s.Id == filters.ServiceId.Value));

            query = filters.SortDirection == SortDirection.Asc ? query.OrderByStringProperty(filters.OrderBy) : query.OrderByStringPropertyDescending(filters.OrderBy);

            return query.Select(Converter);

            return query.ToList().Select(t => new BinGroupDto
            {
                Id = t.Id,
                Name = t.Name,
                Bins = t.Bins.Select(b => new BinDto
                {
                    Name = b.Name,
                    Id = b.Id,
                    Active = b.Active,
                    BankDtoId = b.BankId,
                    CardType = (CardTypeDto)b.CardType,
                    GatewayId = b.GatewayId,
                    BankDto = b.Bank == null ? null : new BankDto
                    {
                        Code = b.Bank.Code,
                        Name = b.Bank.Name,
                    }
                }).ToList()
            }).ToList();
        }

        public override void Edit(BinGroupDto entity)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) && s.Id != entity.Id).Any())
            {
                throw new BusinessException(CodeExceptions.BINGROUP_NAME_USED);
            }

            Repository.ContextTrackChanges = true;

            var dbEntity = ((IRepositoryBinGroup)Repository).GetByIdTracking(entity.Id, x => x.Bins);

            //Para no generar confusiones no permitimos editar el nombre de los grupo de bines default (credito extranjeras, credito nacional, debito extranjeras, etc....)
            if (IsDefaultGroup(entity.Id) && dbEntity.Name != entity.Name)
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.BINGROUP_DEFAULT_CANNOT_BE_RENAMED, dbEntity.Name);
            }

            dbEntity.Name = entity.Name;

            //Borro los bines en la lista de borrados
            foreach (var deletedBinDto in entity.DeletedBins)
            {
                var deletedBin = _binRepository.GetById(deletedBinDto.Id);
                dbEntity.Bins.Remove(deletedBin);
            }

            //Agrego los bines en la lista de borrados
            foreach (var deletedBinDto in entity.AddedBins)
            {
                var addedBin = _binRepository.GetById(deletedBinDto.Id);
                dbEntity.Bins.Add(addedBin);
            }

            dbEntity.Services.Clear();
            entity.Services.ForEach(b => dbEntity.Services.Add(_serviceRepository.All(x => x.Id == b.Id).First()));
           

            Repository.Edit(dbEntity);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public override BinGroupDto Create(BinGroupDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                throw new BusinessException(CodeExceptions.BINGROUP_NAME_USED);
            }

            var newBinGroup = new BinGroup
            {
                Name = entity.Name,
                Bins = new List<Bin>(),
                Services = new List<Service>()
            };

            if (entity.AddedBins != null && entity.AddedBins.Any())
            {
                entity.AddedBins.ForEach(b => newBinGroup.Bins.Add(_binRepository.GetById(b.Id)));
            }

            entity.Services.ForEach(b => newBinGroup.Services.Add(_serviceRepository.All(x => x.Id == b.Id).First()));

            Repository.Create(newBinGroup);
            Repository.Save();

            return returnEntity ? Converter(newBinGroup) : null;
        }

        public override void Delete(Guid id)
        {

            Repository.ContextTrackChanges = true;
            var dbEntity = Repository.GetById(id, x => x.Bins);

            if (IsDefaultGroup(id))
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.BINGROUP_DEFAULT_CANNOT_BE_DELETED, dbEntity.Name);
            }

            dbEntity.Bins.Clear();
            Repository.Save();

            Repository.Delete(id);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public BinGroupDto GetById(Guid id)
        {
            return Converter(Repository.GetById(id));
        }

        private bool IsDefaultGroup(Guid id)
        {
            return id.ToString().ToLower() == ConfigurationManager.AppSettings["CreditoInternacional"].ToLower() ||
                   id.ToString().ToLower() == ConfigurationManager.AppSettings["CreditoNacional"].ToLower() ||
                   id.ToString().ToLower() == ConfigurationManager.AppSettings["DebitoInternacional"].ToLower() ||
                   id.ToString().ToLower() == ConfigurationManager.AppSettings["DebitoNacional"].ToLower() ||
                   id.ToString().ToLower() == ConfigurationManager.AppSettings["PrepagoInternacional"].ToLower() ||
                   id.ToString().ToLower() == ConfigurationManager.AppSettings["PrepagoNacional"].ToLower();
        }
    }
}
