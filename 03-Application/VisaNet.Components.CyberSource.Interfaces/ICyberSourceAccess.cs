using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Components.CyberSource.Interfaces
{
    public interface ICyberSourceAccess
    {
        CyberSourceDataDto GeneratePayment(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData);
        CyberSourceOperationData VoidPayment(CancelPayment cancel);
        CyberSourceOperationData DeleteCard(DeleteCardDto delete);
        CyberSourceOperationData RefundPayment(RefundPayment refund);
        CyberSourceOperationData ReversePayment(RefundPayment reverse);
        Task<List<ConciliationCybersourceDto>> GenerateConciliation(DateTime from, DateTime to);

        CyberSourceMerchantDefinedDataDto LoadMerchantDefinedData(IAssociationInfoDto associatedDto, CyberSourceExtraDataDto cyberSourceExtraData, int paymentsCount);

        CyberSourceOperationData TestPayment(Guid serviceId);
        CyberSourceOperationData TestCancelPayment(Guid serviceId, CyberSourceOperationData cSOperationData, ApplicationUserDto user = null, CardDto card = null);
        CyberSourceOperationData TestReversePayment(Guid serviceId);
        bool TestReports(Guid serviceId);

        IDictionary<string, string> LoadKeysForRegisteredUserPayment(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForRecurrentUserPayment(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForAnonymousUserPayment(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForNewUserPayment(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForToken(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForRegisteredUserTokenApps(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForRecurrentUserToken(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForNewUserTokenApps(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForNewRecurrentUser(IGenerateToken generateToken);

        IDictionary<string, string> LoadKeysForTokenDebitNewUser(IGenerateToken generateToken);
        IDictionary<string, string> LoadKeysForTokenDebitRegisteredUser(IGenerateToken generateToken);
        string GetCardNumberByToken(CybersourceGetCardNameDto dto);
    }
}