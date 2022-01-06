using System;
using System.Configuration;
using System.IO;
using VisaNet.Common.Logging.NLog;
using VisaNet.Utilities.SFTPClient;

namespace VisaNet.ConsoleApplication.FilesShipping
{
    public class SftpTest
    {

        public void TestLoad()
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIO PROCESO PRUEBA SFTP " + DateTime.Now.ToString("G"));

            var path = ConfigurationManager.AppSettings["TESTFILEPATH"];
            var fullPath = Path.Combine(path, ConfigurationManager.AppSettings["TESTFILENAME"]);
            try
            {
                FileStream file = null;
                var fileName = "";
                if (File.Exists(fullPath))
                {
                    file = File.Open(fullPath, FileMode.Open);
                    NLogLogger.LogEvent(NLogType.Info, "    Encontre el archivo en el path: " + fullPath);
                    fileName = file.Name;
                    file.Close();
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, "    No existe el archivo path: " + fullPath);
                    NLogLogger.LogEvent(NLogType.Info, "FIN PROCESO PRUEBA SFTP");
                    return;
                }

                var options = new SftpConfigurationOptions()
                {
                    SshPrivateKeyPath = ConfigurationManager.AppSettings["TESTSshPrivateKeyPath"],
                    SshPrivateKeyName = ConfigurationManager.AppSettings["TESTSshPrivateKeyName"],
                    HostName = ConfigurationManager.AppSettings["TESTSFTPHostName"],
                    PortNumber = int.Parse(ConfigurationManager.AppSettings["TESTSFTPPortNumber"]),
                    SshHostKeyFingerprint = ConfigurationManager.AppSettings["TESTSshHostKeyFingerprint"],
                    UsePassAndCertificate = true,
                    Password = ConfigurationManager.AppSettings["TESTSFTPPassword"],
                    UserName = ConfigurationManager.AppSettings["TESTSFTPUserName"],
                    SessionLogPath = ConfigurationManager.AppSettings["TESTSFTPLogPath"] + @"\SFTP.txt",
                };
                

                var sftpClient = new SftpClient(options);
                NLogLogger.LogEvent(NLogType.Info, "    path: " + path);
                NLogLogger.LogEvent(NLogType.Info, "    name: " + ConfigurationManager.AppSettings["TESTFILENAME"]);
                sftpClient.SendFile(fullPath, ConfigurationManager.AppSettings["TESTFILENAME"]);
                NLogLogger.LogEvent(NLogType.Info, "    Intento realizado de envio SFTP de archivo (" + fileName + ")");

                Testdownload("", ConfigurationManager.AppSettings["TESTFILENAME"], Path.Combine(path,"Download\\"));
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);                
            }
            
            NLogLogger.LogEvent(NLogType.Info, "FIN PROCESO PRUEBA SFTP");
        }

        public void Testdownload(string remotePath, string fileName, string savePath)
        {
            var options = new SftpConfigurationOptions()
            {
                SshPrivateKeyPath = ConfigurationManager.AppSettings["TESTSshPrivateKeyPath"],
                SshPrivateKeyName = ConfigurationManager.AppSettings["TESTSshPrivateKeyName"],
                HostName = ConfigurationManager.AppSettings["TESTSFTPHostName"],
                PortNumber = int.Parse(ConfigurationManager.AppSettings["TESTSFTPPortNumber"]),
                SshHostKeyFingerprint = ConfigurationManager.AppSettings["TESTSshHostKeyFingerprint"],
                UsePassAndCertificate = true,
                Password = ConfigurationManager.AppSettings["TESTSFTPPassword"],
                UserName = ConfigurationManager.AppSettings["TESTSFTPUserName"],
                SessionLogPath = ConfigurationManager.AppSettings["TESTSFTPLogPath"] + @"\SFTP.txt",
            };


            var sftpClient = new SftpClient(options);
            NLogLogger.LogEvent(NLogType.Info, "    fileName: " + fileName);
            NLogLogger.LogEvent(NLogType.Info, "    remotePath: " + remotePath);
            NLogLogger.LogEvent(NLogType.Info, "    savePath: " + savePath);
            sftpClient.GetFile(remotePath, ConfigurationManager.AppSettings["TESTFILENAME"], savePath);
            

        }
    }
}
