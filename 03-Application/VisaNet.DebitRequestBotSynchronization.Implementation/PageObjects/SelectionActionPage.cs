using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects
{
    public class SelectionActionPage : BasePage, ISelectionActionPage
    {
        public SelectionActionPage(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "btnNewRequest")]
        private IWebElement buttonNewRequest;

        [FindsBy(How = How.Id, Using = "btnCancelRequest")]
        private IWebElement buttonCancelRequest;

        public void SelectNewRequestAction()
        {
            buttonNewRequest.Click();            
        }

        public void SelectCancelRequestAction()
        {
            buttonCancelRequest.Click();
        }
    }
}
