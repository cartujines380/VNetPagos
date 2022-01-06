using System;
using System.Collections.Generic;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceCustomerSite
    {
        WebhookAccessTokenDto SendAccessTokenByMail(CustomerSiteGenerateAccessTokenDto dto);
        WebhookAccessTokenDto SendAccessTokenBySms(CustomerSiteGenerateAccessTokenDto dto);
        WebhookAccessTokenDto SendAccessTokenByWhatsapp(CustomerSiteGenerateAccessTokenDto dto);
        WebhookAccessTokenDto GenerateAccessToken(CustomerSiteGenerateAccessTokenDto dto);
        void NewUserEmail(NewUserEmailDto dto);
        void ResetPasswordEmail(ResetPasswordEmailDto dto);
        bool CancelAccessToken(Guid accessTokenId);
        TransactionResult CancelTansaction(CustomerSiteCancelTransactionDto dto);
        IList<CustomerSiteCommerceDto> GetCommercesDebit(List<int> productIds = null, string search = null);
        IList<CustomerSiteCommerceDto> GetCommercesDebitFromCustomerSite(CustomerSiteCommerceFilterDto filterDto);
        CustomerSiteCommerceDto FindCommerceDebit(Guid id);
        void UpdateCommerceDebitCatche();
    }
}