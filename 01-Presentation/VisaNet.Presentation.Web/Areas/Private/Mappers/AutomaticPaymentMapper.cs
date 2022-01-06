using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Areas.Private.Models;

namespace VisaNet.Presentation.Web.Areas.Private.Mappers
{
    public static class AutomaticPaymentMapper
    {

        public static ServiceAssociatedDto ToDto(this AutomaticPaymentModel entity)
        {
            var dto = new ServiceAssociatedDto()
                      {
                          Id = entity.ServiceId,
                          AutomaticPaymentDto = new AutomaticPaymentDto()
                                                {
                                                    DaysBeforeDueDate = entity.DayBeforeExpiration,
                                                    Maximum = entity.MaxAmount,
                                                    Quotas = entity.MaxCountPayments,
                                                    UnlimitedQuotas = entity.UnlimitedQuotas,
                                                    QuotasDone = entity.QuotasDone,
                                                    ServiceAssosiateId = entity.ServiceId,
                                                    SuciveAnnualPatent = entity.SuciveAnnualPatent
                                                },
                          Enabled = true
                      };

            return dto;
        }

        public static AutomaticPaymentModel ToAutomaticPaymentModel(this ServiceAssociatedDto entity)
        {
            var model = new AutomaticPaymentModel()
                        {
                            ServiceId = entity.Id,
                            ServiceReferenceName = entity.ServiceDto.ReferenceParamName,
                            ServiceReferenceName2 = entity.ServiceDto.ReferenceParamName2,
                            ServiceReferenceName3 = entity.ServiceDto.ReferenceParamName3,
                            ServiceReferenceValue = entity.ReferenceNumber,
                            ServiceReferenceValue2 = entity.ReferenceNumber2,
                            ServiceReferenceValue3 = entity.ReferenceNumber3,
                            ServiceReferenceName4 = entity.ServiceDto.ReferenceParamName4,
                            ServiceReferenceName5 = entity.ServiceDto.ReferenceParamName5,
                            ServiceReferenceName6 = entity.ServiceDto.ReferenceParamName6,
                            ServiceReferenceValue4 = entity.ReferenceNumber4,
                            ServiceReferenceValue5 = entity.ReferenceNumber5,
                            ServiceReferenceValue6 = entity.ReferenceNumber6,
                            ServiceName = entity.ServiceDto.Name,
                            DayBeforeExpiration = entity.AutomaticPaymentDto != null ? entity.AutomaticPaymentDto.DaysBeforeDueDate : 0,
                            MaxAmount = entity.AutomaticPaymentDto != null ? entity.AutomaticPaymentDto.Maximum : 0,
                            MaxCountPayments = entity.AutomaticPaymentDto != null ? entity.AutomaticPaymentDto.Quotas : 0,
                            QuotasDone = entity.AutomaticPaymentDto != null ? entity.AutomaticPaymentDto.QuotasDone : 0,
                            UnlimitedQuotas = entity.AutomaticPaymentDto == null || entity.AutomaticPaymentDto.UnlimitedQuotas,
                            DefaultCardMask = entity.DefaultCard != null ? entity.DefaultCard.MaskedNumber : "",
                            SuciveAnnualPatent = entity.AutomaticPaymentDto != null && entity.AutomaticPaymentDto.SuciveAnnualPatent,

                            UnlimitedAmount = entity.AutomaticPaymentDto == null || entity.AutomaticPaymentDto.UnlimitedAmount,
                        };

            return model;
        }
    }
}