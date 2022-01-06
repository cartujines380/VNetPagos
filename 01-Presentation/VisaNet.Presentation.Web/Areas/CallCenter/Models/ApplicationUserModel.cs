using System;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Models
{
    public class ApplicationUserModel
    {
        [CustomDisplay("ApplicationUserDto_Id")]
        public Guid Id { get; set; }

        [CustomDisplay("ApplicationUserDto_Email")]
        public string Email { get; set; }

        [CustomDisplay("ApplicationUserDto_CallCenterKey")]
        public string CallCenterKey { get; set; }

        [CustomDisplay("ApplicationUserDto_Name")]
        public string Name { get; set; }

        [CustomDisplay("ApplicationUserDto_Surname")]
        public string Surname { get; set; }

        [CustomDisplay("ApplicationUserDto_IdentityNumber")]
        public string IdentityNumber { get; set; }

        [CustomDisplay("ApplicationUserDto_MobileNumber")]
        public string MobileNumber { get; set; }

        [CustomDisplay("ApplicationUserDto_PhoneNumber")]
        public string PhoneNumber { get; set; }

        [CustomDisplay("ApplicationUserDto_Address")]
        public string Address { get; set; }
    }
}