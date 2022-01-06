using System;
using System.Configuration;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.DebitRequestBotSynchronization.Implementation
{
    public class DebitRequestBotSynchronizatorService : IDebitRequestBotSynchronizatorService
    {
        private readonly IServiceDebitRequest _serviceDebitRequest;
        private readonly string _debitSiteUrl;
        private readonly string _identityUser;
        private readonly string _passwordUser;

        public DebitRequestBotSynchronizatorService(IServiceDebitRequest serviceDebitRequest)
        {
            _serviceDebitRequest = serviceDebitRequest;
            _debitSiteUrl = ConfigurationManager.AppSettings["debitSiteUrl"];
            _identityUser = ConfigurationManager.AppSettings["identityUser"];
            _passwordUser = ConfigurationManager.AppSettings["passwordUser"];
        }

        public void StartSynchronization()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(_debitSiteUrl);

            try
            {
                Login(driver);

                var requests = _serviceDebitRequest.GetDebitToSync();

                requests.ToList().ForEach(request =>
                {
                    SyncRequest(driver, request);                    
                });
            }
            catch(PageObjectException e)
            {
                NLogLogger.LogEvent(e);
            }
            driver.Close();
        }

        private void Login(IWebDriver driver)
        {
            var loginPage = new LoginPage(driver);
            loginPage.Login(_identityUser, _passwordUser);
        }

        private void SyncRequest(IWebDriver driver, DebitRequestSyncDto request)
        {
            try
            {
                //SELECCION DE ACCION
                var selectionActionPage = new SelectionActionPage(driver);

                if (request.Type == DebitRequestTypeDto.High)
                {
                    selectionActionPage.SelectNewRequestAction();

                    //CARGA DE DATOS DEL USUARIO
                    var userDataPage = new UserDataPage(driver);
                    var user = request.User;

                    userDataPage.SetUserData(user.FullName, user.IdentityNumber, user.Address, "0", "Montevideo", "Montevideo", user.PhoneNumber, user.Email);
                }
                else
                {
                    selectionActionPage.SelectCancelRequestAction();
                }
                
                //CARGA DE PRODUCTO
                var requestPage = new RequestPage(driver);
                requestPage.SelectCommerceGroup(request.MerchantGroupId);
                requestPage.SelectCommerce(request.MerchantId);
                requestPage.SelectService(request.MerchantProductId);
                requestPage.SetCreditCard(request.CardNumber, request.CardMonth, request.CardYear);
                requestPage.SetReferences(request.User.FullName, request.References.OrderBy(x => x.Index).Select(x => x.Value));
                requestPage.ValidateAndNext();

                //CONFIRMACION DE SOLICITUD
                var confirmPage = new ConfirmPage(driver);
                var reference = confirmPage.FinishRequest();

                _serviceDebitRequest.SetRequestSynchronizated(request.Id, reference);
            }
            catch (PageObjectException e)
            {
                NLogLogger.LogEvent(e);
                _serviceDebitRequest.SetRequestErrorSynchronization(request.Id, e.Message);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                _serviceDebitRequest.SetRequestErrorSynchronization(request.Id, e.Message);
                driver.Url = string.Format("{0}/SelectOptionA.aspx", _debitSiteUrl);
            }
        }
    }
}
