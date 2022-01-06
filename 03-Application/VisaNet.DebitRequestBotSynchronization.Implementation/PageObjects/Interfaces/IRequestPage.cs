using System.Collections.Generic;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces
{
    public interface IRequestPage
    {
        void SelectCommerceGroup(int commerceGroupId);
        void SelectCommerce(int commerceId);

        void SelectService(int serviceId);

        void SetCreditCard(string cardNumber, int month, int year);

        void SetReferences(string fullname, IEnumerable<string> references);

        void ValidateAndNext();
    }
}
