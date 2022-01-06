using System;
using System.Collections.Generic;

namespace VisaNet.Presentation.Administration.Models
{
    public class InterpreterModel 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }

        public virtual ICollection<ServiceModel> Services { get; set; }
    }
}
