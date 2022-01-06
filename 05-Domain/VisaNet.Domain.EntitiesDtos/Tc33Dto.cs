using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class Tc33Dto
    {
        public  Guid Id { get; set; }

        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }

        public string Errors { get; set; }
        public Tc33StateDto State { get; set; }

        public ICollection<Tc33TransactionDto> Transactions { get; set; }
        
        //858 = U$ , 840 = USD
        public int TransactionPesosCount { get; set; }
        public int TransactionDollarCount { get; set; }
        public double TransactionPesosAmount { get; set; }
        public double TransactionDollarAmount { get; set; }

        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }


    }
}
