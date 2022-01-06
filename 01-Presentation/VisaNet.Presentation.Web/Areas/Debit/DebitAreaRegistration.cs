using System.Web.Mvc;

namespace VisaNet.Presentation.Web.Areas.Debit
{
    public class DebitAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Debit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Debit",
                "Debit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}