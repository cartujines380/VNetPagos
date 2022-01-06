using System;

namespace VisaNet.Presentation.Administration.Models
{
    public class RunConciliationModel
    {
        public DateTime Date { get; set; }
        public DateTime DateTo { get; set; }
        public string FileName { get; set; }
        public ConciliationAppModel? App { get; set; }
    }

    //Se debe mapear con ConciliationAppDto
    public enum ConciliationAppModel
    {
        //Site = 0,  // Site no se usa para la funcionalidad de correr conciliación manual
        CyberSource = 1,
        Banred = 2,
        Sistarbanc = 3,
        Sucive = 4,
        //Tc33 = 5, // TC33 no se usa para la funcionalidad de correr conciliación manual
        Batch = 6,
    }

}