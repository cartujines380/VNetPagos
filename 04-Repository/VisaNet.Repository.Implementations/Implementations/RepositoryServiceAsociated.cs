using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryServiceAsociated : BaseRepository<ServiceAssociated>, IRepositoryServiceAsociated
    {
        public RepositoryServiceAsociated(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {

        }

        public void ChangeState(Object[] data)
        {
            ContextTrackChanges = true;
            var id = (String)data[0];
            var state = (bool)data[1];

            var entityDb = GetById(Guid.Parse(id), s => s.DefaultCard);

            if (state && !entityDb.DefaultCard.Active)
            {
                ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.SERVICE_DEFAULT_CARD_NOTACTIVE);
            }

            entityDb.Active = state;
            if (data.Length == 3)
            {
                entityDb.Enabled = (bool)data[2];
            }

            Edit(entityDb);
            Save();

            ContextTrackChanges = false;
        }

        public void ChangeState(IList<ServiceAssociated> servicesAssociated, bool active, bool? enable)
        {
            ContextTrackChanges = true;
            foreach (var serviceAssociated in servicesAssociated)
            {
                var entityDb = GetById(serviceAssociated.Id, s => s.DefaultCard);

                if (active && !entityDb.DefaultCard.Active)
                {
                    continue;
                }

                entityDb.Active = active;
                if (enable.HasValue)
                {
                    entityDb.Enabled = enable.Value;
                }
                Edit(entityDb);
            }
            Save();
            ContextTrackChanges = false;
        }

        public IQueryable<WebServiceApplicationClient> AssosiatedServiceClientUpdate(WebServiceClientInput entity)
        {
            var cc = entity.CodCommerce.ToString();
            var cb = entity.CodBranch.ToString();
            var query = AllNoTracking(null, x => x.Service, x => x.Service.ServiceGateways);

            query = query.Where(x => x.Service.ServiceGateways.Any(s => s.ReferenceId.Equals(cc) && s.ServiceType.Equals(cb)));

            if (entity.FechaDesde != new DateTime())
            {
                query = query.Where(x => x.LastModificationDate >= entity.FechaDesde);
            }
            if (!string.IsNullOrEmpty(entity.RefCliente1))
            {
                query = query.Where(x => x.ReferenceNumber.ToLower().Equals(entity.RefCliente1.ToLower()));
            }
            if (!string.IsNullOrEmpty(entity.RefCliente2))
            {
                query = query.Where(x => x.ReferenceNumber2.ToLower().Equals(entity.RefCliente2.ToLower()));
            }
            if (!string.IsNullOrEmpty(entity.RefCliente3))
            {
                query = query.Where(x => x.ReferenceNumber3.ToLower().Equals(entity.RefCliente3.ToLower()));
            }
            if (!string.IsNullOrEmpty(entity.RefCliente4))
            {
                query = query.Where(x => x.ReferenceNumber4.ToLower().Equals(entity.RefCliente4.ToLower()));
            }
            if (!string.IsNullOrEmpty(entity.RefCliente5))
            {
                query = query.Where(x => x.ReferenceNumber5.ToLower().Equals(entity.RefCliente5.ToLower()));
            }
            if (!string.IsNullOrEmpty(entity.RefCliente6))
            {
                query = query.Where(x => x.ReferenceNumber6.ToLower().Equals(entity.RefCliente6.ToLower()));
            }
            return query.Select(x => new WebServiceApplicationClient()
            {
                FchModificacion = x.LastModificationDate,
                RefCliente1 = x.ReferenceNumber,
                RefCliente2 = x.ReferenceNumber2,
                RefCliente3 = x.ReferenceNumber3,
                RefCliente4 = x.ReferenceNumber4,
                RefCliente5 = x.ReferenceNumber5,
                RefCliente6 = x.ReferenceNumber6,

                UserId = x.RegisteredUserId,
                Estado = x.Active ? 1 : 0
            });
        }
    }
}
