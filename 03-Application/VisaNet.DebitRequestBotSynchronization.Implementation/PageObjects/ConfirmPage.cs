using System;
using OpenQA.Selenium;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using VisaNet.Common.Exceptions;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects
{
    public class ConfirmPage : BasePage, IConfirmPage
    {
        public ConfirmPage(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "btnSend")]
        private IWebElement buttonSend;

        [FindsBy(How = How.Id, Using = "lblRef")]
        private IWebElement labelReference;

        public int FinishRequest()
        {
            try
            {
                GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id(buttonSend.GetAttribute("id"))));
            }
            catch (Exception e)
            {
                buttonBack.Click();
                throw new PageObjectException(string.Format("{0}: {1}", "La solicitud ya existe para la alta o no existe para la baja", e.Message));
            }

            buttonSend.Click();

            try
            {
                GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id(labelReference.GetAttribute("id"))));
                var nroReferencia = int.Parse(labelReference.Text);
                buttonBack.Click();
                return nroReferencia;
            }
            catch (Exception e)
            {
                buttonBack.Click();
                throw new PageObjectException(string.Format("{0}: {1}", "Error al finalizar sincronización", e.Message));
            }
        }
    }
}
