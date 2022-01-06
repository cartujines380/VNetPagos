namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces
{
    public interface IUserDataPage
    {
        void SetUserData(string fullname, string identityNumber, string streetAddress, string numberAddress, string city, string location, string phone, string email);
    }
}
