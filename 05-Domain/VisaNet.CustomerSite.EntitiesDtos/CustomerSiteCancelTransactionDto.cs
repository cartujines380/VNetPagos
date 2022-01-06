using System;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteCancelTransactionDto
    {
        public Guid IdService { get; set; }
        public string IdOperation { get; set; }
        public string IdOperationTransaction { get; set; }
    }

}
