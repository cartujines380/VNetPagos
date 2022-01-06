using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class RunConciliationDto
    {
        public ConciliationAppDto App { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTo { get; set; }
        public string FileName { get; set; }
    }
}