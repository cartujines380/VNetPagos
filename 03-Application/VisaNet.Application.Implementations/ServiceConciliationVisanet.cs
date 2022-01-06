using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationVisanet : BaseService<ConciliationVisanet, ConciliationVisanetDto>, IServiceConciliationVisanet
    {
        private readonly IRepositoryConciliationVisanet _repositoryConciliationVisanet;

        public ServiceConciliationVisanet(IRepositoryConciliationVisanet repository)
            : base(repository)
        {
            _repositoryConciliationVisanet = repository;
        }

        public override IQueryable<ConciliationVisanet> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationVisanetDto Converter(ConciliationVisanet entity)
        {
            var dto = new ConciliationVisanetDto
            {
                Id = entity.Id,
                Date = entity.Date,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                RequestId = entity.RequestId,
                ConciliationType = (ConciliationTypeDto)entity.ConciliationType
            };

            return dto;
        }

        public override ConciliationVisanet Converter(ConciliationVisanetDto entity)
        {
            var cBanred = new ConciliationVisanet
            {
                Id = entity.Id,
                Date = entity.Date,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                RequestId = entity.RequestId,
                ConciliationType = (ConciliationType)entity.ConciliationType
            };
            return cBanred;
        }

        public override void Edit(ConciliationVisanetDto entity)
        {
            Repository.ContextTrackChanges = true;

            var entity_db = Repository.All().FirstOrDefault(e => e.RequestId.Equals(entity.RequestId));

            entity_db.Amount = entity.Amount;
            entity_db.BillExternalId = entity.BillExternalId;
            entity_db.ConciliationType = (ConciliationType)((int)entity.ConciliationType);
            entity_db.Currency = entity.Currency;
            entity_db.Date = entity.Date;

            Repository.Edit(entity_db);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

    }
}