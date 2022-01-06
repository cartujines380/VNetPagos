using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Microsoft.Reporting.WebForms;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServicePaymentTicket : IServicePaymentTicket
    {
        private readonly IRepositoryPayment _repositoryPayment;

        public ServicePaymentTicket(IRepositoryPayment repositoryPayment)
        {
            _repositoryPayment = repositoryPayment;
        }

        public void GeneratePaymentTicket(string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType)
        {
            var payment = _repositoryPayment.AllNoTracking(p => p.TransactionNumber == transactionNumber,
                                    p => p.Service,
                                    p => p.Bills,
                                    p => p.Card,
                                    p => p.PaymentIdentifier,
                                    p => p.ServiceAssosiated,
                                    p => p.Gateway,
                                    p => p.DiscountObj)
                            .FirstOrDefault();

            if (payment == null)
            {
                throw new FatalException(CodeExceptions.PAYMENT_NOT_FOUND);
            }
            if (payment.Bills == null || !payment.Bills.Any())
            {
                throw new FatalException(CodeExceptions.PAYMENT_NO_BILL);
            }

            if(userId != Guid.Empty)
            {
                if (payment.RegisteredUserId.HasValue && payment.RegisteredUserId.Value != userId)
                {
                    throw new FatalException(CodeExceptions.PAYMENT_USER_NOT_RELATED);
                }
                if (payment.AnonymousUserId.HasValue && payment.AnonymousUserId.Value != userId)
                {
                    throw new FatalException(CodeExceptions.PAYMENT_USER_NOT_RELATED);
                }
            }

            
            var path = Path.Combine(ConfigurationManager.AppSettings["TicketTemplateUrl"], "PaymentTicket.rdlc");

            var localReport = new LocalReport { ReportPath = path };
            var sec = new PermissionSet(PermissionState.Unrestricted);
            localReport.SetBasePermissionsForSandboxAppDomain(sec);

            localReport.SetParameters(new ReportParameter("ServiceName", payment.Service.Name + (payment.ServiceAssosiated != null ? (!String.IsNullOrEmpty(payment.ServiceAssosiated.Description) ? " - " + payment.ServiceAssosiated.Description : "") : "")));
            localReport.SetParameters(new ReportParameter("Date", payment.Date.ToString("dd/MM/yyyy")));
            localReport.SetParameters(new ReportParameter("Time", payment.Date.ToString("HH:mm:ss")));
            localReport.SetParameters(new ReportParameter("TransactionNumber", payment.TransactionNumber));
            localReport.SetParameters(new ReportParameter("Amount", string.Format("{0} {1}", payment.Currency, payment.TotalAmount.ToString("##,#0.00", CultureInfo.CurrentCulture))));
            localReport.SetParameters(new ReportParameter("BillExternalId", payment.Bills.FirstOrDefault().BillExternalId));
            localReport.SetParameters(new ReportParameter("Discount", string.Format("{0} {1}", payment.Currency, payment.Discount.ToString("##,#0.00", CultureInfo.CurrentCulture))));
            localReport.SetParameters(new ReportParameter("TotalWithDiscount", string.Format("{0} {1}", payment.Currency, (payment.TotalAmount - payment.Discount).SignificantDigits(2).ToString("##,#0.00", CultureInfo.CurrentCulture))));
            localReport.SetParameters(new ReportParameter("PagoImporte", payment.Gateway.Enum == (int)GatewayEnumDto.Importe ? "0" : "1"));
            localReport.SetParameters(new ReportParameter("PagoApp", payment.Gateway.Enum == (int)GatewayEnumDto.Apps ? "0" : "1"));
            localReport.SetParameters(new ReportParameter("CardNumber", payment.Card.MaskedNumber));
            localReport.SetParameters(new ReportParameter("Quota", payment.Quotas.ToString()));    

            var discountType = "Descuento: ";
            if (payment.DiscountObj != null)
            {
                discountType = String.Concat(EnumHelpers.GetName(typeof(DiscountLabelTypeDto), (int)payment.DiscountObj.DiscountLabel, EnumsStrings.ResourceManager), ": ");
            }

            localReport.SetParameters(new ReportParameter("DiscountType", discountType));

            var data = new DataTable();
            data.Columns.AddRange(new[] { new DataColumn("Reference"), new DataColumn("ReferenceValue") });

            if (payment.Gateway.Enum != (int)GatewayEnumDto.Apps)
            {
                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName) && !string.IsNullOrEmpty(payment.ReferenceNumber))
                    data.Rows.Add(payment.Service.ReferenceParamName, payment.ReferenceNumber);

                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName2) && !string.IsNullOrEmpty(payment.ReferenceNumber2))
                    data.Rows.Add(payment.Service.ReferenceParamName2, payment.ReferenceNumber2);

                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName3) && !string.IsNullOrEmpty(payment.ReferenceNumber3))
                    data.Rows.Add(payment.Service.ReferenceParamName3, payment.ReferenceNumber3);

                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName4) && !string.IsNullOrEmpty(payment.ReferenceNumber4))
                    data.Rows.Add(payment.Service.ReferenceParamName4, payment.ReferenceNumber4);

                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName5) && !string.IsNullOrEmpty(payment.ReferenceNumber5))
                    data.Rows.Add(payment.Service.ReferenceParamName5, payment.ReferenceNumber5);

                if (!string.IsNullOrEmpty(payment.Service.ReferenceParamName6) && !string.IsNullOrEmpty(payment.ReferenceNumber6))
                    data.Rows.Add(payment.Service.ReferenceParamName6, payment.ReferenceNumber6);
            }

            localReport.DataSources.Add(new ReportDataSource("DataTableReferences", data));
            const string reportType = "PDF";
            string encoding;
            string fileNameExtension;
            const string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
            //var deviceInfo = "<DeviceInfo><PageHeight>11.7in</PageHeight><PageWidth>8.3in</PageWidth><OutputFormat>PDF</OutputFormat></DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
        }
    }
}