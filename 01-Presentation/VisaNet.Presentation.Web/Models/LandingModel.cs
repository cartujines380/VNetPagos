using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Models
{
    public class LandingModel
    {
        public List<DeptoModel> States { get; set; }
        
    }

    public class DeptoModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImgName { get; set; }
        public bool Active { get; set; }
    }
    
}
