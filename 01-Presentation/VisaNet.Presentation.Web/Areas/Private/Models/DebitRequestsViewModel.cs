using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class DebitRequestsViewModel
    {
        public DebitRequestsViewModel()
        {
            Filters = new DebitRequestsFilterDto();
        }

        public DebitRequestsFilterDto Filters { get; set; }

        public List<SelectListItem> Cards { get; set; }

        public IEnumerable<DebitRequestTableModel> Requests { get; set; }

        public Guid DebitRequestIdToCancel { get; set; }
    }
}