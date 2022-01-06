namespace VisaNet.Domain.Entities
{
    public class Dashboard
    {
        public int PaymentType { get; set; }
        public double Amount{ get; set; }
        public int Count { get; set; }

        public int NotificationQuantity { get; set; }
        public int ContactQuantity { get; set; }
        public int ComplaintContactQuantity { get; set; }
        public int QuestionContactQuantity { get; set; }
        public int SuggestionContactQuantity { get; set; }
        public int OtherContactQuantity { get; set; }

        public int UserQuantity { get; set; }

        public int SubscriberQuantity { get; set; }
    }
}
