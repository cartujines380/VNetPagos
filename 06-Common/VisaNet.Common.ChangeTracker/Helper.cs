using System;

namespace VisaNet.Common.ChangeTracker
{
    public static class Helper
    {
        private const string ProxyNamespace = @"System.Data.Entity.DynamicProxies";

        public static Type GetEntityType(Type entityType)
        {
            if (entityType.Namespace == ProxyNamespace)
            {
                return GetEntityType(entityType.BaseType);
            }
            else
            {
                return entityType;
            }
        }
    }
}
