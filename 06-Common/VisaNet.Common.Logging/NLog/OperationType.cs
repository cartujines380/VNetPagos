namespace VisaNet.Common.Logging.NLog
{
    public enum OperationType
    {
        Unknown = 1,

        CyberSourcePost = 2,
        CyberSourceAcknowledgement = 3,
        CyberSourceVoidProcess = 4,

        DebitRequestProcessDataCybersource = 5,
        DebitRequestCancelDebit = 6,
        DebitRequestSetSynchronizated = 7,
        DebitRequestSetErrorSynchronization = 8,

        GetBills = 9,

        //TODO: seguir agregando más, y cuando se invoque a los Logs recordar setearlo
    }
}