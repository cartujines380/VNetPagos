using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ReportsModel;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationSummary : BaseRepository<ConciliationSummary>, IRepositoryConciliationSummary
    {
        private readonly Database _db;

        public RepositoryConciliationSummary(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _db = context.Database;
        }

        public IEnumerable<T> ExecuteSQL<T>(string sql, object[] parameters)
        {
            return parameters == null ? _db.SqlQuery<T>(sql) : _db.SqlQuery<T>(sql, parameters);
        }

        public void GenerateSummary()
        {
            try
            {
                const string sql = "StoreProcedure_VisaNet_ConciliationSummaryProcess";
                var connectionString = ConfigurationManager.ConnectionStrings["AppContext"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    int commandTimeout;
                    if (!int.TryParse(ConfigurationManager.AppSettings["AppContext"], out commandTimeout))
                    {
                        commandTimeout = 1800; // =30 min (si no se ejecuta por un tiempo demora un rato)
                    }

                    var command = new SqlCommand(sql, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = commandTimeout
                    };

                    #region Set up parameters
                    command.Parameters.Add("@result", SqlDbType.Int, 1).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@resulttext", SqlDbType.VarChar, 1).Direction = ParameterDirection.Output;
                    #endregion

                    // Open connection and execute stored procedure
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        throw;
                    }

                    var result = Convert.ToInt32(command.Parameters["@result"].Value);
                    var desc = Convert.ToString(command.Parameters["@resulttext"].Value);

                    connection.Close();

                    NLogLogger.LogEvent(NLogType.Info, "    El Store Procedure StoreProcedure_VisaNet_ConciliationSummaryProcess finalizo " + (result == 0 ? "OK" : "CON ERRORES"));
                    NLogLogger.LogEvent(NLogType.Info, "    Mensaje: " + desc);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "    Error en ejecucción del Store Procedure StoreProcedure_VisaNet_ConciliationSummaryProcess");
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        public IList<ReportConciliationDetail> ObtainTableInfo(DateTime from, DateTime to, string requestId = null,
            string uniqueIdenfifier = null, string serviceId = null, string email = null, string state = null,
            string gatewayName = null, string column = null, string comment = null, string order = "DESC",
            string displayLength = "1", string displayStart = "0")
        {
            try
            {
                var result = _context.Database.SqlQuery<ReportConciliationDetail>(
                   "StoreProcedure_VisaNet_ConciliationDetail " +
                   "@inputDateFrom = {0}" +
                   ", @inputDateTo = {1}" +
                   ", @inputCsTrnsNumber = {2}" +
                   ", @inputGatewayTrnsNumber = {3}" +
                   ", @inputServiceId = {4}" +
                   ", @inputEmail = {5}" +
                   ", @inputState = {6}" +
                   ", @inputcolumn = {7}" +
                   ", @inputComment = {8}" +
                   ", @inputOrder = {9}" +
                   ", @inputDisplayLength = {10}" +
                   ", @inputDisplayStart = {11}" +
                   ", @inputCount = {12}"
                   , from //{0}
                   , to //{1}
                   , requestId //{2}
                   , uniqueIdenfifier //{3}
                   , serviceId //{4}
                   , email //{5}
                   , state //{6}
                   , column //{7}
                   , comment //{8}
                   , order //{9}
                   , displayLength //{10}
                   , displayStart //{11}
                   , false); //{12}

                var list = result.ToList();

                var countResult = _context.Database.SqlQuery<int>(
                   "StoreProcedure_VisaNet_ConciliationDetail " +
                   "@inputDateFrom = {0}" +
                   ", @inputDateTo = {1}" +
                   ", @inputCsTrnsNumber = {2}" +
                   ", @inputGatewayTrnsNumber = {3}" +
                   ", @inputServiceId = {4}" +
                   ", @inputEmail = {5}" +
                   ", @inputState = {6}" +
                   ", @inputcolumn = {7}" +
                   ", @inputComment = {8}" +
                   ", @inputOrder = {9}" +
                   ", @inputDisplayLength = {10}" +
                   ", @inputDisplayStart = {11}" +
                   ", @inputCount = {12}"
                   , from //{0}
                   , to //{1}
                   , requestId //{2}
                   , uniqueIdenfifier //{3}
                   , serviceId //{4}
                   , email //{5}
                   , state //{6}
                   , column //{7}
                   , comment //{8}
                   , order //{9}
                   , displayLength //{10}
                   , displayStart //{11}
                   , true); //{12}

                if (list.FirstOrDefault() != null)
                {
                    list.FirstOrDefault().RowsCount = countResult.First();
                }

                return list;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "    Error en ejecucción del Store Procedure StoreProcedure_VisaNet_ConciliationDetail");
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

    }
}