using System;

namespace VisaNet.Presentation.Administration.Models
{
    public class ReportsConciliationRunViewModel
    {
        public Guid Id { get; set; }

        public string CreationDate { get; set; }
        public string LastModificationDate { get; set; }

        public string App { get; set; }
        public string RunType { get; set; } //indica si es una corrida automática o que se hizo manualmente desde el Admin

        public string State { get; set; }

        public string InputFileName { get; set; }
        public string ConciliationDateFrom { get; set; }
        public string ConciliationDateTo { get; set; }

        //Estos campos quedan para ver dentro del detalle
        public string ResultDescription { get; set; }
        public string ExceptionMessage { get; set; }

    }
}