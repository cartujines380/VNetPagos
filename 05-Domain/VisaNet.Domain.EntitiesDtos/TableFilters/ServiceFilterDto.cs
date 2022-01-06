using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    using System;

    public class ServiceFilterDto : BaseFilter
    {
        public ServiceFilterDto()
        {
            OrderBy = "Name";
        }

        public Guid ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceCategory { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public string ServiceContainerName { get; set; }
        public string ServiceContainerId { get; set; }
        public bool Active { get; set; }
        public string State { get; set; }
        public int Gateway { get; set; }

        public bool? IsContainer { get; set; }
        public bool WithoutServiceInContainer { get; set; }
        //public bool OnlyToAssociate { get; set; }
        //public bool OnlyToPay{ get; set; }
        //public bool? ServiceWithOutContainerAndContainer { get; set; }
        
        public bool EnablePublicPayment { get; set; }
        public bool EnablePrivatePayment { get; set; }
        public bool EnableAssociation { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"Description",Description},
                {"ServiceCategory",ServiceCategory},
                {"ServiceContainerName",ServiceContainerName},
                {"ServiceContainerId",ServiceContainerId},
                {"NActiveame",Active.ToString()},
                {"Gateway",Gateway.ToString()},

                {"State",State},
                {"IsContainer",IsContainer.ToString()},
                {"WithServiceInContainer",WithoutServiceInContainer.ToString()},
                {"EnablePublicPayment",EnablePublicPayment.ToString()},
                {"EnablePrivatePayment",EnablePrivatePayment.ToString()},
                {"EnableAssociation",EnableAssociation.ToString()},
                {"ServiceCategoryId",ServiceCategoryId.ToString()},
                
                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
