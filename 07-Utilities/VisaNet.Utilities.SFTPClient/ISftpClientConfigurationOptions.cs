namespace VisaNet.Utilities.SFTPClient
{
    public interface ISftpClientConfigurationOptions
    {
        string SshPrivateKeyPath { get; set; }
        string SshPrivateKeyName { get; set; }
        string Password { get; set; }
        string HostName { get; set; }
        int PortNumber { get; set; }
        string UserName { get; set; }
        string SshHostKeyFingerprint { get; set; }
        string SessionLogPath { get; set; }
        bool UsePassAndCertificate { get; set; }
    }
}
