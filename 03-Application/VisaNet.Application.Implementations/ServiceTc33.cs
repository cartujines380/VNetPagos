using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Repository;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ComplexTypes;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.ExtensionMethods;
using SortDirection = VisaNet.Domain.EntitiesDtos.TableFilters.SortDirection;

namespace VisaNet.Application.Implementations
{
    public class ServiceTc33 : BaseService<Tc33, Tc33Dto>, IServiceTc33
    {
        private readonly ILoggerRepository _repositoryLogPaymentCyberSource;
        private readonly IServiceTc33Transaction _serviceTc33Transaction;

        public ServiceTc33(IRepositoryTc33 repository, ILoggerRepository repositoryLogPaymentCyberSource, IServiceTc33Transaction serviceTc33Transaction)
            : base(repository)
        {
            _repositoryLogPaymentCyberSource = repositoryLogPaymentCyberSource;
            _serviceTc33Transaction = serviceTc33Transaction;
        }

        public IEnumerable<Tc33Dto> GetDataForTable(ReportsTc33FilterDto filters)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filters.CreationDateFromString))
            {
                from = DateTime.Parse(filters.CreationDateFromString, new CultureInfo("es-UY"));
            }
            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filters.CreationDateToString))
            {
                to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY"));
            }
            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate >= from);
            }
            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate <= dateTo);
            }

            if (!string.IsNullOrEmpty(filters.InputFileName))
                query = query.Where(sc => sc.InputFileName.ToLower().Contains(filters.InputFileName.ToLower()));

            if (!string.IsNullOrEmpty(filters.Transaction))
                query =
                    query.Where(
                        sc => sc.Transactions.Any(c => c.RequestId.ToLower().Contains(filters.Transaction.ToLower()))
                              || sc.Errors.ToLower().Contains(filters.Transaction.ToLower()));

            #region Sort By

            switch (filters.OrderBy)
            {
                case "0":
                    query = filters.SortDirection == SortDirection.Asc
                        ? query.OrderBy(p => p.CreationDate)
                        : query.OrderByDescending(p => p.CreationDate);
                    break;
                case "1":
                    query = filters.SortDirection == SortDirection.Asc
                        ? query.OrderBy(p => p.InputFileName)
                        : query.OrderByDescending(p => p.InputFileName);
                    break;
                default:
                    query = filters.SortDirection == SortDirection.Asc
                        ? query.OrderBy(p => p.CreationDate)
                        : query.OrderByDescending(p => p.CreationDate);
                    break;
            }

            #endregion

            var queryPaged = query.Skip(filters.DisplayStart).Take((int)filters.DisplayLength);

            return queryPaged.Select(t => new Tc33Dto
            {
                Id = t.Id,
                InputFileName = t.InputFileName,
                OutputFileName = t.OutputFileName,
                CreationDate = t.CreationDate,
                CreationUser = t.CreationUser,
                State = (Tc33StateDto)(int)t.State,
                Errors = t.Errors
            }).ToList();
        }

        public int GetDataForTableCount(ReportsTc33FilterDto filters)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filters.CreationDateFromString))
            {
                from = DateTime.Parse(filters.CreationDateFromString, new CultureInfo("es-UY"));
            }
            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filters.CreationDateToString))
            {
                to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY"));
            }
            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate >= from);
            }
            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate <= dateTo);
            }

            if (!string.IsNullOrEmpty(filters.InputFileName))
                query = query.Where(sc => sc.InputFileName.ToLower().Contains(filters.InputFileName.ToLower()));

            if (!string.IsNullOrEmpty(filters.Transaction))
                query =
                    query.Where(
                        sc => sc.Transactions.Any(c => c.RequestId.ToLower().Contains(filters.Transaction.ToLower()))
                              || sc.Errors.ToLower().Contains(filters.Transaction.ToLower()));


            return query.Select(t => new Tc33Dto
            {
                Id = t.Id,
            }).Count();
        }

        public override IQueryable<Tc33> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override Tc33Dto Converter(Tc33 entity)
        {
            return new Tc33Dto
            {
                CreationDate = entity.CreationDate,
                InputFileName = entity.InputFileName,
                OutputFileName = entity.OutputFileName,
                State = (Tc33StateDto)(int)entity.State,
                Transactions =
                    entity.Transactions != null
                        ? entity.Transactions.Select(x => _serviceTc33Transaction.Converter(x)).ToList()
                        : null,
                Id = entity.Id,
                CreationUser = entity.CreationUser,
                Errors = entity.Errors,
                TransactionDollarAmount = entity.TransactionDollarAmount,
                TransactionDollarCount = entity.TransactionDollarCount,
                TransactionPesosAmount = entity.TransactionPesosAmount,
                TransactionPesosCount = entity.TransactionPesosCount,
            };
        }

        public override Tc33 Converter(Tc33Dto entity)
        {
            var tc33 = new Tc33
            {
                InputFileName = entity.InputFileName,
                OutputFileName = entity.OutputFileName,
                State = (Tc33State)(int)entity.State,
                //Transactions = entity.Transactions.Select(x => _serviceTc33Transaction.Converter(x)).ToList(),
                Id = entity.Id,
                Errors = entity.Errors,
                TransactionDollarAmount = entity.TransactionDollarAmount,
                TransactionDollarCount = entity.TransactionDollarCount,
                TransactionPesosAmount = entity.TransactionPesosAmount,
                TransactionPesosCount = entity.TransactionPesosCount
            };
            return tc33;
        }

        public Tc33Dto Create(Tc33Dto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            var old =
                Repository.All(x => x.InputFileName.Equals(entity.InputFileName) && x.State == Tc33State.Process)
                    .FirstOrDefault();
            if (old != null)
                Repository.Delete(old.Id);

            var efEntity = Converter(entity);

            Repository.Create(efEntity);
            Repository.Save();

            if (entity.Transactions != null && entity.Transactions.Any())
            {
                foreach (var transaction in entity.Transactions)
                {
                    transaction.Tc33Id = efEntity.Id;
                    _serviceTc33Transaction.Create(transaction);
                }
            }
            Repository.ContextTrackChanges = false;
            return returnEntity ? Converter(efEntity) : null;
        }

        public void Edit(Tc33Dto entity)
        {
            try
            {
                Repository.ContextTrackChanges = true;
                var efEntity = Repository.GetById(entity.Id);
                efEntity.OutputFileName = entity.OutputFileName;
                efEntity.State = (Tc33State)(int)entity.State;
                efEntity.TransactionDollarAmount = entity.TransactionDollarAmount;
                efEntity.TransactionDollarCount = entity.TransactionDollarCount;
                efEntity.TransactionPesosAmount = entity.TransactionPesosAmount;
                efEntity.TransactionPesosCount = entity.TransactionPesosCount;
                efEntity.Errors = entity.Errors;

                Repository.Edit(efEntity);
                Repository.Save();
                Repository.ContextTrackChanges = false;
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(exception);
                Repository.ContextTrackChanges = false;
                throw;
            }
        }

        public IList<string> GetTC33Transactions(Guid id)
        {
            return GetById(id, x => x.Transactions).Transactions.Select(x => x.RequestId).ToList();
        }

        public void DownloadDetails(Guid id, out byte[] renderedBytes, out string mimeType)
        {
            var tc33 = GetById(id);

            if (tc33 == null)
            {
                throw new FatalException(CodeExceptions.TC33_NOT_FOUND);
            }

            //solo sabemos las tarjetas de los pagos registardos.
            var path = Path.Combine(ConfigurationManager.AppSettings["TicketTemplateUrl"], "Tc33Details.rdlc");

            var localReport = new LocalReport { ReportPath = path };

            var sec = new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            localReport.SetBasePermissionsForSandboxAppDomain(sec);

            localReport.SetParameters(new ReportParameter("Date", tc33.CreationDate.ToString("dd/MM/yyyy")));
            localReport.SetParameters(new ReportParameter("PesosCount", tc33.TransactionPesosCount.ToString()));
            localReport.SetParameters(new ReportParameter("PesosAmount", tc33.TransactionPesosAmount.ToString()));
            localReport.SetParameters(new ReportParameter("DollarCount", tc33.TransactionDollarCount.ToString()));
            localReport.SetParameters(new ReportParameter("DollarAmount", tc33.TransactionDollarAmount.ToString()));
            localReport.SetParameters(new ReportParameter("InputFileName", tc33.InputFileName));
            localReport.SetParameters(new ReportParameter("OutputFileName", tc33.OutputFileName));
            localReport.SetParameters(new ReportParameter("PrintDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

            const string reportType = "PDF";
            string encoding;
            string fileNameExtension;
            const string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
            //var deviceInfo = "<DeviceInfo><PageHeight>11.7in</PageHeight><PageWidth>8.3in</PageWidth><OutputFormat>PDF</OutputFormat></DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
        }

        public LogDto GetLogFromDb(string requestId, int intento)
        {
            try
            {
                if (intento < 5)
                {
                    var sql =
                        "SELECT lc.TransactionType AS 'TransactionType', lc.CyberSourceLogData_TransactionId AS 'TransactionId'," +
                        " lc.CyberSourceLogData_AuthTime AS 'AuthTime', lc.CyberSourceLogData_AuthCode AS 'AuthCode'," +
                        " lc.CyberSourceLogData_AuthAmount AS 'AuthAmount', lc.CyberSourceLogData_ReqCurrency AS 'ReqCurrency'," +
                        " lc.CyberSourceLogData_BillTransRefNo AS 'BillTransRefNo', lc.CyberSourceLogData_ReqAmount AS 'ReqAmount', " +
                        "lc.TransactionDateTime AS 'TransactionDateTime'" +
                        "FROM Logs l " +
                        "LEFT JOIN LogPaymentCyberSources lc ON lc.id = l.LogPaymentCyberSource_Id " +
                        "WHERE lc.CyberSourceLogData_TransactionId = @Nrotrn AND (lc.TransactionType = @TrnsType OR lc.TransactionType = @TrnsType2)";

                    var list = _repositoryLogPaymentCyberSource.ExecuteSQL<LogCs>(sql,
                        new[]
                        {
                            new SqlParameter("@Nrotrn", requestId),
                            new SqlParameter("@TrnsType", (int) TransactionType.Payment),
                            new SqlParameter("@TrnsType2", (int) TransactionType.Refund)
                        }).ToList();

                    if (list.Any())
                    {
                        var first = list.FirstOrDefault();
                        var log = new LogDto()
                        {
                            LogPaymentCyberSource = new LogPaymentCyberSourceDto()
                            {
                                TransactionDateTime = first.TransactionDateTime,
                                CyberSourceLogData = new CyberSourceLogDataDto()
                                {
                                    AuthAmount = first.AuthAmount,
                                    AuthTime = first.AuthTime,
                                    TransactionId = first.TransactionId,
                                    AuthCode = first.AuthCode,
                                    ReqCurrency = first.ReqCurrency,
                                    BillTransRefNo = first.BillTransRefNo,
                                    ReqAmount = first.ReqAmount,
                                    TransactionType = (TransactionType)first.TransactionType,
                                }
                            }
                        };
                        return log;
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(NLogType.Error, "Excepction en Intento " + intento);
                NLogLogger.LogTc33Event(exception);
                if (intento < 5)
                {
                    intento = intento + 1;
                    return GetLogFromDb(requestId, intento);
                }
            }
            return null;
        }

        public bool WasAlreadyProccessed(string requestId)
        {
            try
            {
                if (!string.IsNullOrEmpty(requestId))
                {
                    const string sql = "SELECT count(1) FROM Tc33Transactions t WHERE t.RequestId = @Nrotrn";
                    var list =
                        _serviceTc33Transaction.ExecuteSql<int>(sql, new[] { new SqlParameter("@Nrotrn", requestId) })
                            .ToList();
                    if (list.FirstOrDefault() > 0)
                    {
                        NLogLogger.LogTc33Event(NLogType.Info, "Ya existe " + requestId);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogTc33Event(NLogType.Info, "wasAlreadyProccessed error");
                NLogLogger.LogTc33Event(exception);
                throw;
            }
            return false;
        }

    }
}