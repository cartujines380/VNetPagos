using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebCardClientService : WebApiClientService, IWebCardClientService
    {
        public WebCardClientService(ITransactionContext transactionContext)
            : base("WebCard", transactionContext)
        {

        }

        public Task<ICollection<CardDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CardDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<CardDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CardDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<CardDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        public Task<CardDto> FindByToken(String token)
        {
            return
                WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestGet(
                    BaseUri + "/GetByToken", TransactionContext, new Dictionary<string, string> { { "token", token } }));
        }

        public Task<CardDto> Create(CardDto entity)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }

        public Task Edit(CardDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, entity.Id, entity));
        }

        public Task ActivateCard(CardOperationDto cardOperationDto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(
               BaseUri + "/ActivateCard", TransactionContext, cardOperationDto));
        }

        public Task DesactivateCard(CardOperationDto cardOperationDto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(
                BaseUri + "/DesactivateCard", TransactionContext, cardOperationDto));
        }

        public Task EliminateCard(CardOperationDto cardOperationDto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(
                BaseUri + "/EliminateCard", TransactionContext, cardOperationDto));
        }

        public Task MigrateServices(Guid userId, Guid oldCardId, Guid newCardId)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(
                BaseUri + "/MigrateServices", TransactionContext, new CardMigrationServicesDto()
                {
                    ApplicationUserId = userId,
                    NewCardId = newCardId,
                    OldCardId = oldCardId
                }
                                                                  ));
        }

        public Task<CardDto> GenerateExternalId(CardOperationDto dto)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestPost(
                BaseUri + "/GenerateExternalId", TransactionContext, dto));
        }

        public Task<CardDto> FindWithServices(Guid cardId)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestGet(
                 BaseUri + "/FindWithServices", TransactionContext, new Dictionary<string, string> { { "cardId", cardId.ToString() } }));
        }

        public Task<CardMigrationTestDto> TestMigration(Guid oldCardId, Guid newCardId, Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<CardMigrationTestDto>(new WebApiHttpRequestPost(BaseUri + "/TestMigration", TransactionContext, new CardMigrationServicesDto
            {
                ApplicationUserId = userId,
                NewCardId = newCardId,
                OldCardId = oldCardId
            }
            ));
        }

        public Task<ICollection<ServiceAssociatedDto>> GetAssociatedServices(Guid cardId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetAssociatedServices", TransactionContext, new Dictionary<string, string> { { "cardId", cardId.ToString() } }));
        }

        public Task<ICollection<DebitAssociatedDto>> GetAssociatedDebits(Guid cardId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<DebitAssociatedDto>>(new WebApiHttpRequestGet(BaseUri + "/GetAssociatedDebits", TransactionContext, new Dictionary<string, string> { { "cardId", cardId.ToString() } }));
        }

        public Task<string> GetQuotasForBin(int cardBin)
        {
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestGet(BaseUri + "/GetQuotasForBin", TransactionContext,
                new Dictionary<string, string>
                {
                    { "cardBin", cardBin.ToString() }
                }));
        }

        public Task<string> GetQuotasForBinAndService(int cardBin, Guid serviceId)
        {
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestGet(BaseUri + "/GetQuotasForBinAndService", TransactionContext,
                new Dictionary<string, string>
                {
                    { "cardBin", cardBin.ToString() },
                    { "serviceId", serviceId.ToString() }
                }));
        }

        public Task EditCardDescription(Guid cardId, string description)
        {

            var cardDto = new CardDto { Id = cardId, Description = description };

            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(
                BaseUri + "/EditCardDescription", TransactionContext, cardDto));
        }
    }
}