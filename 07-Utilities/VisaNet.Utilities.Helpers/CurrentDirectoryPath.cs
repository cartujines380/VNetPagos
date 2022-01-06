using System;
using System.IO;
using System.Reflection;

namespace VisaNet.Utilities.Helpers
{
    public static class CurrentDirectoryPath
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path).Replace("bin", string.Empty);
            }
        } 
    }
}
