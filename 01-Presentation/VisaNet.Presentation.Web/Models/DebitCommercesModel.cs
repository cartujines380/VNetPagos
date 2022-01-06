using System;
using System.Collections.Generic;

namespace VisaNet.Presentation.Web.Models
{
    public class DebitCommercesModel
    {
        public IList<CommerceModel> SelectedCommerces { get; set; }
        public DateTime Date { get; set; }
    }
}