using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Hubs;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class DebitSuscriptionController : BaseController
    {
        private readonly IDebitCommerceClientService _debitCommerceClientService;
        private readonly ICustomerSiteCommerceClientService _customerSiteCommerceClientService;
        private readonly ICyberSourceAccessClientService _cyberSourceAccessClientService;
        private readonly IEmailService _emailService;
        
        public DebitSuscriptionController(
            IDebitCommerceClientService debitCommerceClientService, 
            ICustomerSiteCommerceClientService customerSiteCommerceClientService,
            ICyberSourceAccessClientService cyberSourceAccessClientService,
            IEmailService emailService)
        {
            _debitCommerceClientService = debitCommerceClientService;
            _customerSiteCommerceClientService = customerSiteCommerceClientService;
            _cyberSourceAccessClientService = cyberSourceAccessClientService;
            _emailService = emailService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.DebitSuscriptionList)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthentication(Actions.DebitSuscriptionList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerDebitSuscriptionList(Request, param);

            var suscriptions = await _debitCommerceClientService.GetDebitSuscriptionList(filter);
            var suscriptionsCount = await _debitCommerceClientService.GetDebitSuscriptionListCount(filter);

            var dataModel = suscriptions.Select(d => new 
            {
                Id = d.Id,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy hh:mmm:ss"),
                UserEmail = d.ApplicationUserDto != null ? d.ApplicationUserDto.Email : string.Empty,
                UserId = d.UserId,
                MaskedNumber = d.CardDto != null ? d.CardDto.MaskedNumber : string.Empty,                
                CardId = d.CardId,
                CommerceName = d.CommerceDto != null ? d.CommerceDto.Name : string.Empty,
                CommerceId = d.CommerceDto != null ? d.CommerceDto.Id.ToString() : string.Empty,
                DebitStateId = (int)d.State,
                DebitState = EnumHelpers.GetName(typeof(DebitRequestStateDto), (int)d.State, EnumsStrings.ResourceManager),
                DebitTypeId = (int)d.Type,
                DebitType = EnumHelpers.GetName(typeof(DebitRequestTypeDto), (int)d.Type, EnumsStrings.ResourceManager),
                ProductName = d.CommerceDto != null && d.CommerceDto.ProductosListDto != null && d.CommerceDto.ProductosListDto.FirstOrDefault() != null ? 
                    d.CommerceDto.ProductosListDto.FirstOrDefault().Description : string.Empty,
                ProductId = d.CommerceDto != null && d.CommerceDto.ProductosListDto != null && d.CommerceDto.ProductosListDto.FirstOrDefault() != null ? 
                    d.CommerceDto.ProductosListDto.FirstOrDefault().Id.ToString() : string.Empty,
                ReferenceNumber = d.ReferenceNumber,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = suscriptionsCount,
                iTotalDisplayRecords = suscriptionsCount,
                aaData = dataModel
            }, JsonRequestBehavior.AllowGet);            
        }

        [HttpGet]
        [CustomAuthentication(Actions.DebitSuscriptionDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            var debit = await _debitCommerceClientService.Find(id);
            var commerce = await _debitCommerceClientService.FindCommerce(debit.DebitProductId);
            
            var commerceModel = commerce.ToCommerceModel();
            var debitModel = debit.ToModel();
            debitModel.CommerceModel = commerceModel;
            debitModel.ReferenceNumber = debit.ReferenceNumber;

            return View("Details", debitModel);
        }

        [HttpGet]
        [CustomAuthentication(Actions.DebitSuscriptionDetails)]
        public async Task<ActionResult> ExcelExport()
        {
            await Task.Run(() => GenerateDebitExcel((SystemUserDto)Session[SessionConstants.CURRENT_USER]));
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        #region Private methods

        private async void GenerateDebitExcel(SystemUserDto userSession)
        {
            try
            {
                var data = await _debitCommerceClientService.ExcelExportManualSynchronization();

                var requestGroupByCyberSource = data.GroupBy(x => new { x.MerchantId, x.MerchantReferenceCode, x.Token });
                
                foreach(var requestGroup in requestGroupByCyberSource.AsParallel())
                {
                    var requestSelected = requestGroup.First();
                    var dto = new CybersourceGetCardNameDto
                    {
                        MerchantId = requestSelected.MerchantId,
                        MerchantReferenceCode = requestSelected.MerchantReferenceCode,
                        Token = requestSelected.Token
                    };

                    var cardNumber = await _cyberSourceAccessClientService.GetCardNumberByToken(dto);

                    foreach(var request in requestGroup)
                    {
                        request.CardNumber = cardNumber;
                    }
                }

                var headers = new[]
                    {
                    PresentationAdminStrings.Debit_CreationDate,
                    PresentationAdminStrings.Debit_SynchronizationDate,
                    PresentationAdminStrings.Debit_UserFullName,
                    PresentationAdminStrings.Debit_UserIdentityNumber,
                    PresentationAdminStrings.Debit_UserAddress,
                    PresentationAdminStrings.Debit_UserPhoneNumber,
                    PresentationAdminStrings.Debit_UserEmail,
                    PresentationAdminStrings.Debit_MerchantGroup,
                    PresentationAdminStrings.Debit_Merchant,
                    PresentationAdminStrings.Debit_MerchantProduct,
                    PresentationAdminStrings.Debit_type,
                    PresentationAdminStrings.Debit_CardNumber,
                    PresentationAdminStrings.Debit_CardMonth,
                    PresentationAdminStrings.Debit_CardYear,
                    PresentationAdminStrings.Debit_References
                };

                var memoryStream = ExcelExporter.ExcelExport("DébitosSincronizaciónManual", data, headers);
                var path = ConfigurationManager.AppSettings["ManualSynchronizationFolder"];
                var filename = string.Format("{0}_{1}.{2}", "DébitosSincronizaciónManual", DateTime.Now.ToString("yyyyMMddhhmmss"), "csv");
                var fullpath = System.IO.Path.Combine(path, filename);

                var fileStream = System.IO.File.Create(fullpath, memoryStream.Length);

                await fileStream.WriteAsync(memoryStream, 0, memoryStream.Length);

                GenerateNotification(userSession, string.Format("Se ha generado el archivo {0} en el directorio {1}", filename, path), NotificationType.Success);

            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                GenerateNotification(userSession, "error al generar archivo", NotificationType.Error, e);
            }
        }

        private void GenerateNotification(SystemUserDto userSession, string message, NotificationType type, Exception e = null)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(userSession.Id.ToString()).notify(message, type.ToString());

            var dto = new DebitManualSyncNotificationDto
            {
                UserName = userSession.LDAPUserName,
                Type = type,
                ExpectionError = e,
                Message = message
            };

            _emailService.SendNotificationManualSynchronization(dto);
        }

        #endregion Private methods
    }
}