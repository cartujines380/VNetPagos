using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BillGeocomDto
    {
        public Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public string Codigo { get; set; }
        //separado por ; si hay mas de una linea
        public string Line { get; set; }
        public string Year { get; set; }
        public string Allowed { get; set; }

        public bool Payable { get; set; }

        public List<BillGeocomDto> Details { get; set; }

        public int IdPadron { get; set; }

        public string GeocomPreBillNumber { get; set; }
        public string DashboardDescription { get; set; }

    }
}
