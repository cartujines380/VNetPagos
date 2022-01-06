using System.Collections.Generic;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class DashBoardModel
    {
        public List<BillToPayModel> Bills { get; set; }
        public ICollection<NextAutomaticPaymentModel> AutoPayments { get; set; }
    }

}
