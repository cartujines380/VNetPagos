using System;
using System.IO;
using VisaNet.Common.Logging.NLog;
using WinSCP;

namespace VisaNet.Utilities.SFTPClient
{
    public class SftpClient
    {
        private readonly ISftpClientConfigurationOptions _configurationOptions;
        
        public SftpClient(ISftpClientConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        public void SendFile(string filePath, string fileName)
        {
            SessionOptions sessionOptions = CreateSessionOptionsBasedOnConfiguration();
            Session session = null;
            var isOpen = false;
            try
            {                               
                //Send Ftp Files - same idea as above - try...catch and try to repeat this code 
                //if you can't connect the first time, timeout after a certain number of tries. 
                //Send Ftp Files - same idea as above - try...catch and try to repeat this code 
                //if you can't connect the first time, timeout after a certain number of tries. 
                session = new Session
                {
                    
                    SessionLogPath = _configurationOptions.SessionLogPath,
                    
                };
             
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                isOpen = true;
                //Get Ftp File
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    FilePermissions = null,             //Can set user, Group, or other Read/Write/Execute permissions. 
                    PreserveTimestamp = false,          //Set last write time of destination file to that of source file - basically change the timestamp to match destination and source files.                                           
                };

                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                var transferResult = session.PutFiles(filePath, fileName, false, transferOptions);

                //Throw on any error 
                transferResult.Check();
                //Log information and break out if necessary  
                session.Close();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("   SftpClient - SendFile file path: {0} file name: {1}, - Excepcion", filePath, fileName));
                NLogLogger.LogEvent(exception);
                if (isOpen && session != null) session.Close();
                throw exception;
            }
        }
        
        public void GetFile(string filePath, string fileName, string localfilePath)
        {
            SessionOptions sessionOptions = CreateSessionOptionsBasedOnConfiguration();
            Session session = null;
            var isOpen = false;
            var remotePath = Path.Combine(filePath , fileName);
            var localPath = Path.Combine(localfilePath, fileName);
            
            try
            {
                using (session = new Session() { SessionLogPath = _configurationOptions.SessionLogPath })
                {
                    session.Open(sessionOptions);
                    isOpen = true;
                    session.GetFiles(session.EscapeFileMask(remotePath), localPath).Check();
                    session.Close();
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, string.Format("   SftpClient - GetFile filepath: {0} filename: {1}, localfilePath: {2}, - Excepcion", filePath, fileName, localfilePath));
                NLogLogger.LogEvent(exception);
                if (isOpen && session != null) session.Close();
                throw exception;
            }
        }
        
        private SessionOptions CreateSessionOptionsBasedOnConfiguration()
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = _configurationOptions.HostName,
                PortNumber = _configurationOptions.PortNumber,
                UserName = _configurationOptions.UserName,
                GiveUpSecurityAndAcceptAnySshHostKey = true,
            };

            if (_configurationOptions.UsePassAndCertificate)
            {
                sessionOptions.SshHostKeyFingerprint = _configurationOptions.SshHostKeyFingerprint;

                string keyPath = Path.Combine(_configurationOptions.SshPrivateKeyPath, _configurationOptions.SshPrivateKeyName);
                sessionOptions.SshPrivateKeyPath = keyPath;

                sessionOptions.Password = _configurationOptions.Password;

            }else if (string.IsNullOrEmpty(_configurationOptions.Password))
            {
                sessionOptions.SshHostKeyFingerprint = _configurationOptions.SshHostKeyFingerprint;

                string keyPath = Path.Combine(_configurationOptions.SshPrivateKeyPath, _configurationOptions.SshPrivateKeyName);
                sessionOptions.SshPrivateKeyPath = keyPath;
            }
            else
            {
                sessionOptions.Password = _configurationOptions.Password;
            }

            return sessionOptions;
        }
    }
}
