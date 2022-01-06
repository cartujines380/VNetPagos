using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using VisaNet.Common.Exceptions;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base
{
    public abstract class BasePage
    {
        protected readonly IWebDriver _driver;

        public BasePage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(_driver, this);
        }

        [FindsBy(How = How.Id, Using = "lblError")]
        protected IWebElement labelError;

        [FindsBy(How = How.Id, Using = "btnNext")]
        protected IWebElement buttonNext;

        [FindsBy(How = How.Id, Using = "btnBack")]
        protected IWebElement buttonBack;

        protected WebDriverWait GetWait(int? time = 10)
        {
            return new WebDriverWait(_driver, TimeSpan.FromSeconds(time.Value));
        }

        public void SelectDropdownValue(IWebElement element, string value)
        {
            var dropdown  = new SelectElement(element);
            dropdown.SelectByValue(value);
        }

        public virtual void ValidatePage()
        {
            string errorMessage;
            try
            {
                GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id(labelError.GetAttribute("id"))));
                errorMessage = labelError.Text;
            }
            catch(Exception)
            {
                return;
            }
            buttonBack.Click();
            throw new PageObjectException(errorMessage);
        }
    }
}
