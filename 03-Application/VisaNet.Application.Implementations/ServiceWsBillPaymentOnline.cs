using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceWsBillPaymentOnline : BaseService<WsBillPaymentOnline, WsBillPaymentOnlineDto>, IServiceWsBillPaymentOnline
    {
        public ServiceWsBillPaymentOnline(IRepositoryWsBillPaymentOnline repository)
            : base(repository)
        {
        }

        public override IQueryable<WsBillPaymentOnline> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WsBillPaymentOnlineDto Converter(WsBillPaymentOnline entity)
        {
            if (entity == null) return null;

            var auxFields = new Collection<AuxiliarFieldDto>();
            if (entity.AuxiliarFields != null)
            {
                foreach (var field in entity.AuxiliarFields)
                {
                    auxFields.Add(new AuxiliarFieldDto
                    {
                        Id = field.Id,
                        IdValue = field.IdValue,
                        Value = field.Value,
                    });
                }
            }

            var model = new WsBillPaymentOnlineDto
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdOperation = entity.IdOperation,
                BillNumber = entity.BillNumber,
                CodCommerce = entity.CodCommerce,
                CodBranch = entity.CodBrunch,
                IdMerchant = entity.IdMerchant,
                AmountTaxed = entity.AmountTaxed,
                AmountTotal = entity.AmountTotal,
                ConsFinal = entity.ConsFinal,
                Currency = entity.Currency,
                DateBill = entity.DateBill,
                Description = entity.Description,
                IdApp = entity.IdApp,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                Indi = entity.Indi,
                Quota = entity.Quota,
                AuxiliarFields = auxFields,
                PaymentId = entity.PaymentId.HasValue ? entity.PaymentId.Value : Guid.Empty,
                DeviceFingerprint = entity.DeviceFingerprint,
                CustomerIp = entity.CustomerIp,
                CustomerPhone = entity.CustomerPhone,
                WcfVersion = entity.WcfVersion,
                CreationDate = entity.CreationDate,
                CustomerShippingAddresDto = new CustomerShippingAddresDto()
                {
                    Street = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Street : string.Empty,
                    Corner = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Corner : string.Empty,
                    DoorNumber = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.DoorNumber : string.Empty,
                    Longitude = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Longitude : string.Empty,
                    Latitude = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Latitude : string.Empty,
                    Phone = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Phone : string.Empty,
                    PostalCode = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.PostalCode : string.Empty,
                    Complement = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Complement : string.Empty,
                    Neighborhood = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Neighborhood : string.Empty,
                    City = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.City : string.Empty,
                    Country = entity.CustomerShippingAddres != null ? entity.CustomerShippingAddres.Country : string.Empty,
                }
            };

            if (entity.Payment != null)
            {
                model.PaymentDto = new PaymentDto()
                {
                    Id = entity.PaymentId.Value,
                    TransactionNumber = entity.Payment.TransactionNumber,
                    Date = entity.Payment.Date
                };
            }

            return model;
        }

        public override WsBillPaymentOnline Converter(WsBillPaymentOnlineDto entity)
        {
            if (entity == null) return null;

            var auxFields = new Collection<AuxiliarField>();
            if (entity.AuxiliarFields != null)
            {
                foreach (var field in entity.AuxiliarFields)
                {
                    auxFields.Add(new AuxiliarField
                    {
                        Id = field.Id,
                        IdValue = field.IdValue,
                        Value = field.Value,
                    });
                }
            }

            var model = new WsBillPaymentOnline
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdOperation = entity.IdOperation,
                BillNumber = entity.BillNumber,
                CodCommerce = entity.CodCommerce,
                CodBrunch = entity.CodBranch,
                IdMerchant = entity.IdMerchant,
                AmountTaxed = entity.AmountTaxed,
                AmountTotal = entity.AmountTotal,
                ConsFinal = entity.ConsFinal,
                Currency = entity.Currency,
                DateBill = entity.DateBill,
                Description = entity.Description,
                IdApp = entity.IdApp,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                Indi = entity.Indi,
                Quota = entity.Quota,
                AuxiliarFields = auxFields,
                PaymentId = entity.PaymentId,
                DeviceFingerprint = entity.DeviceFingerprint,
                CustomerIp = entity.CustomerIp,
                CustomerPhone = entity.CustomerPhone,
                WcfVersion = entity.WcfVersion,
                CustomerShippingAddres = new CustomerShippingAddres()
                {
                    Street = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Street : string.Empty,
                    Corner = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Corner : string.Empty,
                    DoorNumber = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.DoorNumber : string.Empty,
                    Longitude = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Longitude : string.Empty,
                    Latitude = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Latitude : string.Empty,
                    Phone = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Phone : string.Empty,
                    PostalCode = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.PostalCode : string.Empty,
                    Complement = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Complement : string.Empty,
                    Neighborhood = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Neighborhood : string.Empty,
                    City = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.City : string.Empty,
                    Country = entity.CustomerShippingAddresDto != null ? entity.CustomerShippingAddresDto.Country : string.Empty,
                }
            };
            return model;
        }

        public override WsBillPaymentOnlineDto Create(WsBillPaymentOnlineDto entity, bool returnEntity = false)
        {

            Repository.ContextTrackChanges = true;
            var efEntity = Converter(entity);

            try
            {
                efEntity.GenerateNewIdentity();
                Repository.Create(efEntity);
                Repository.Save();
            }
            catch (DbUpdateException exception)
            {
                NLogLogger.LogEvent(exception);
                if (exception.Message.Contains("IX_IdApp_IdOperation") ||
                (exception.InnerException != null && exception.InnerException.Message.Contains("IX_IdApp_IdOperation")) ||
                (exception.InnerException.InnerException != null && exception.InnerException.InnerException.Message.Contains("IX_IdApp_IdOperation")))
                {
                    //IDOPERACION REPETIDO
                    throw new BusinessException(CodeExceptions.OPERATION_ID_REPETED);
                }
            }
            Repository.ContextTrackChanges = false;
            return returnEntity ? GetById(efEntity.Id) : null;
        }

        public override void Edit(WsBillPaymentOnlineDto dto)
        {
            Repository.ContextTrackChanges = true;

            var entity = Repository.GetById(dto.Id);

            if (entity != null)
            {
                entity.Description = dto.Description;
                entity.Codresult = dto.Codresult;
                entity.PaymentId = dto.PaymentId != null && dto.PaymentId != Guid.Empty ? dto.PaymentId : null;

                Repository.Edit(entity);
                Repository.Save();
            }

            Repository.ContextTrackChanges = false;
        }

        public ICollection<WsBillPaymentOnlineDto> GetBillPaymentsOnlineForTable(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate.CompareTo(dateTo) <= 0);
            }

            if (!filterDto.IdOperation.IsEmpty())
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            if (!filterDto.TransactionNumber.IsEmpty())
                query = query.Where(x => x.PaymentId != Guid.Empty && x.Payment.TransactionNumber.Equals(filterDto.TransactionNumber, StringComparison.OrdinalIgnoreCase));

            //ordeno, skip y take
            query = filterDto.SortDirection == SortDirection.Desc ? query.OrderByDescending(x => x.CreationDate) : query.OrderBy(x => x.CreationDate);
            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            var result = query.Select(t => new WsBillPaymentOnlineDto
            {
                Id = t.Id,
                IdOperation = t.IdOperation,
                IdApp = t.IdApp,
                AmountTaxed = t.AmountTaxed,
                AmountTotal = t.AmountTotal,
                //AuxiliarFields = t.AuxiliarFields,
                BillNumber = t.BillNumber,
                CodBranch = t.CodBrunch,
                CodCommerce = t.CodCommerce,
                Codresult = t.Codresult,
                IdMerchant = t.IdMerchant,
                ConsFinal = t.ConsFinal,
                Currency = t.Currency,
                DateBill = t.DateBill,
                Description = t.Description,
                IdCard = t.IdCard,
                IdUser = t.IdUser,
                Indi = t.Indi,
                Quota = t.Quota,
                WcfVersion = t.WcfVersion,
                CreationDate = t.CreationDate
            }).ToList();

            return result;
        }

        public int GetBillPaymentsOnlineForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate.CompareTo(dateTo) <= 0);
            }

            if (!filterDto.IdOperation.IsEmpty())
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            return query.Select(t => new WsBillPaymentOnlineDto
            {
                Id = t.Id
            }).Count();
        }

        public WsBillPaymentOnlineDto GetByIdOperation(string idOperation, string idApp)
        {
            //VERSION VIEJA DEL WCF NO TIENE IDAPP. CUANDO TODOS CAMBIEN A VERSION NUEVA, ELIMINAR ESTE CONTROL
            var entities = string.IsNullOrEmpty(idApp) ?
                Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase), x => x.AuxiliarFields).ToList() :
                Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase) &&
                x.IdApp.Equals(idApp, StringComparison.InvariantCultureIgnoreCase), x => x.AuxiliarFields).ToList();

            if (entities == null || !entities.Any() || entities.Count > 1)
                return null;

            var entity = entities.FirstOrDefault();

            return Converter(entity);

        }

        public bool IsOperationIdRepited(string idOperation, string idApp)
        {
            var isOperationIdRepited =
               Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase) &&
                   x.IdApp.Equals(idApp, StringComparison.InvariantCultureIgnoreCase))
                   .Any();

            return isOperationIdRepited;
        }

    }
}