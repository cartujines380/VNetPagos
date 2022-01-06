using System;
using System.Configuration;

namespace VisaNet.Common.ConfigSections
{
    public static class EnvironmentConfig
    {
        private const string ProductionString = "live";

        public static bool IsProductionEnvironment
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["CsEnvironment"], ProductionString, StringComparison.CurrentCultureIgnoreCase);
            }
        }

    }
}