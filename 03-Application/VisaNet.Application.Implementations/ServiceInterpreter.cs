using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceInterpreter : BaseService<Interpreter, InterpreterDto>, IServiceInterpreter
    {
        public ServiceInterpreter(IRepositoryInterpreter repository)
            : base(repository)
        {
            
        }

        public override IQueryable<Interpreter> GetDataForTable()
        {
            return Repository.All();
        }
        
        public override InterpreterDto Converter(Interpreter entity)
        {
            var interpretor = new InterpreterDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
            };

            if (entity.Services != null && entity.Services.Any())
            {
                interpretor.Services = entity.Services.Select(x => new ServiceDto()
                {
                    Active = x.Active,
                    Name = x.Name,
                    Id = x.Id,
                }).ToList();
            }

            return interpretor;
        }

        public override Interpreter Converter(InterpreterDto entity)
        {
            var interpretor = new Interpreter()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
            };

            if (entity.Services != null && entity.Services.Any())
            {
                interpretor.Services = entity.Services.Select(x => new Service()
                {
                    Active = x.Active,
                    Name = x.Name,
                    Id = x.Id,
                }).ToList();
            }

            return interpretor;
        }

        public IList<InterpreterDto> GetDataForTable(InterpreterFilterDto filterDto)
        {
            var query = GetDataForInterpreter(filterDto);
            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("0"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }
            else
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }

            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            return query.ToList().Select(Converter).ToList();
        }

        public int GetDataForInterpreterCount(InterpreterFilterDto filterDto)
        {
            var result = GetDataForInterpreter(filterDto);
            return result.Count();
        }

        private IQueryable<Interpreter> GetDataForInterpreter(InterpreterFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filterDto.Name))
                query = query.Where(x => x.Name.Contains(filterDto.Name));

            if (!string.IsNullOrEmpty(filterDto.FileName))
            {
                Guid fileName;
                var fileNameOk = Guid.TryParse(filterDto.FileName, out fileName);
                if(fileNameOk)
                    query = query.Where(x => x.Id == fileName);
            }

            return query;
        }

        public InterpreterDto Create(InterpreterDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.INTERPRETER_NAME_USED);
            }
            return base.Create(entity, returnEntity);
        }

        public override void Edit(InterpreterDto entity)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.INTERPRETER_NAME_USED);
            }

            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id);

            entityDb.Name = entity.Name;
            entityDb.Description = entity.Description;
            
            Repository.Edit(entityDb);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }
    }
}
