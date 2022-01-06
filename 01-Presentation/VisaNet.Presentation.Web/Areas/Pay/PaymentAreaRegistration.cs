using System.Web.Mvc;

namespace VisaNet.Presentation.Web.Areas.Pay
{
    public class PaymentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Pay";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Pay",
                "Pay/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}