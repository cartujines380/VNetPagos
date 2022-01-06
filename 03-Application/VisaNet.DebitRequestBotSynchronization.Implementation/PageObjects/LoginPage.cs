using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects
{
    public class LoginPage : BasePage, ILoginPage
    {
        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "txtUser")]
        private IWebElement userInput;

        [FindsBy(How = How.Id, Using = "txtPass")]
        private IWebElement passwordInput;

        [FindsBy(How = How.Id, Using = "btnLogin")]
        private IWebElement buttonLogin;

        public void Login(string identityUser, string password)
        {
            userInput.SendKeys(identityUser);
            passwordInput.SendKeys(password);
            buttonLogin.Click();
            ValidatePage();            
        }
    }
}
