using System;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class Tc33Model
    {
        public Guid Id { get; set; }

        [CustomDisplay("Tc33Dto_InputFileName")]
        public string InputFileName { get; set; }

        [CustomDisplay("Tc33Dto_OutputFileName")]
        public string OutputFileName { get; set; }

        [CustomDisplay("Tc33Dto_State")]
        public string State { get; set; }
        public int StateValue { get; set; }

        [CustomDisplay("Tc33Dto_CreationDate")]
        public string CreationDate { get; set; }

        [CustomDisplay("Tc33Dto_CreationUser")]
        public string CreationUser { get; set; }

        public string TransactionNotFound { get; set; }
    }

    public class Tc33DetailTrns
    {
        public string Label { get; set; }
        public string Value{ get; set; }
    }
}