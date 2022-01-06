namespace VisaNet.Domain.Entities.Enums
{
    public enum HighwayEmailStatus
    {
        Processing = 0,
        Accepted = 1,
        RejectedFileNotFound = 2,
        RejectedFileNotOne = 3,
        RejectedFileNameError = 4,
        RejectedFileBadlyFormed = 5,
        RejectedSenderNotRegistered = 6,
        RejectedFileNameBadlyFormed = 7,
        RejectedFileType = 8,
        RejectedServiceNotFound = 9,
        ProcessingError = 10,
        RejectedCommitFailed = 11,
    }
}
