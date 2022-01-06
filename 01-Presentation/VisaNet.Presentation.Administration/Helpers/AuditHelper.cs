using VisaNet.Common.Resource.Audit;

namespace VisaNet.Presentation.Administration.Helpers
{
    public static class AuditHelper
    {
        /// <summary>
        /// Translates the table names into an user friendly name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string TranslateAuditTableName(string tableName)
        {
            return AuditResources.ResourceManager.GetString(tableName);
        }
    }
}