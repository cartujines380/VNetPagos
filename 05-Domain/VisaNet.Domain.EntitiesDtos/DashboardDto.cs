namespace VisaNet.Domain.EntitiesDtos
{
    public class DashboardDto
    {
        public int userQuantity { get; set; }

        public int totalQuantity { get; set; }
        public double totalAmount { get; set; }

        public int notRegisteredQuantity { get; set; }
        public double notRegisteredAmount { get; set; }

        public int manualQuantity { get; set; }
        public double manualAmount { get; set; }

        public int automaticQuantity { get; set; }
        public double automaticAmount { get; set; }

        public int appsQuantity { get; set; }
        public double appsAmount { get; set; }

        public int bankQuantity { get; set; }
        public double bankAmount { get; set; }

        public int notificationQuantity { get; set; }

        public int contactQuantity { get; set; }
        public int complaintContactQuantity { get; set; }
        public int questionContactQuantity { get; set; }
        public int suggestionContactQuantity { get; set; }
        public int otherContactQuantity { get; set; }

        public int subscriberQuantity { get; set; }
    }
}
