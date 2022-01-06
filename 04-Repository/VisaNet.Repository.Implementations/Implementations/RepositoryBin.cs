using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryBin : BaseRepository<Bin>, IRepositoryBin
    {
        public RepositoryBin(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public Bin GetDefaultBin()
        {
            var defaultBinValue = int.Parse(ConfigurationManager.AppSettings["DefaultBin"]);

            return AllNoTracking(b => b.Value == defaultBinValue, b => b.Bank).FirstOrDefault();
        }

        public void ExecuteBinManagerSp_insert(string name, int value, string gatewayId, int cardType, int authorizationAmountType, string country, string issuerBin, string processorBin, string user, string groupBinId, bool editedInBo)
        {
            var id = Guid.NewGuid();
            try
            {
                _context.Database.ExecuteSqlCommand(
                "StoreProcedure_VisaNet_BinManager_Create @id, @gatewayId, @cardType, @value, @name, @authorizationAmountType, @country, @issuerBin, @processorBin, @user, @groupBinId, @editedInBo ",
                new SqlParameter("@id", id),
                new SqlParameter("@gatewayId", gatewayId),
                new SqlParameter("@cardType", cardType),
                new SqlParameter("@value", value),
                new SqlParameter("@name", name),
                new SqlParameter("@authorizationAmountType", authorizationAmountType),
                new SqlParameter("@country", country),
                new SqlParameter("@issuerBin", issuerBin),
                new SqlParameter("@processorBin", processorBin),
                new SqlParameter("@user", user),
                new SqlParameter("@groupBinId", groupBinId),
                new SqlParameter("@editedInBo", editedInBo));
            }
            catch (Exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ExecuteBinManagerSp_insert Excepcion. Info: id: {0}, name: {1}, gatewayId: {2}, cardType: {03}, authorizationAmountType: {04},country: {5}," +
                                                 "issuerBin: {06},processorBin: {07},user: {08},groupBinId: {09},editedInBo: {10}, value: {11}.",
                                                 id, name, gatewayId, cardType, authorizationAmountType, country, issuerBin, processorBin, user, groupBinId, editedInBo, value));
                throw;
            }
            
        }

        public void ExecuteBinManagerSp_Update(Guid id, string name, string gatewayId, int cardType, int authorizationAmountType, string country, string issuerBin, string processorBin, string user, string groupBinId, string oldDefaultGroupId, bool editedInBo)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("StoreProcedure_VisaNet_BinManager_Update @id, @gatewayId, @cardType, @name, @authorizationAmountType, @country, @issuerBin, @processorBin, @user, @groupBinId, @oldDefaultGroupId, @editedInBo ",
               new SqlParameter("@id", id),
               new SqlParameter("@gatewayId", gatewayId),
               new SqlParameter("@cardType", cardType),
               new SqlParameter("@name", name),
               new SqlParameter("@authorizationAmountType", authorizationAmountType),
               new SqlParameter("@country", country),
               new SqlParameter("@issuerBin", issuerBin),
               new SqlParameter("@processorBin", processorBin),
               new SqlParameter("@user", user),
               new SqlParameter("@oldDefaultGroupId", oldDefaultGroupId),
               new SqlParameter("@groupBinId", groupBinId),
               new SqlParameter("@editedInBo", editedInBo));
            }
            catch (Exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ExecuteBinManagerSp_Update Excepcion. Info: id: {0}, name: {1}, gatewayId: {2}, cardType: {03}, authorizationAmountType: {04},country: {5}," +
                                                                 "issuerBin: {06},processorBin: {07},user: {08},groupBinId: {09},oldDefaultGroupId: {10},editedInBo: {11}.",
                                                                 id, name, gatewayId, cardType, authorizationAmountType, country, issuerBin, processorBin, user, groupBinId, oldDefaultGroupId, editedInBo));
                throw;
            }
            
        }

        public void ExecuteBinManagerSp_Delete(Guid id)
        {
            _context.Database.ExecuteSqlCommand("StoreProcedure_VisaNet_BinManager_Delete @id", new SqlParameter("@id", id));
        }
    }
}
