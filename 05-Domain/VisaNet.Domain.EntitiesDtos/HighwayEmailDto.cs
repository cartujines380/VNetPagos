using System;
using System.Collections.Generic;
using System.Web;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class HighwayEmailDto
    {
        
        public Guid Id { get; set; }

        public string Sender { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string AttachmentInputName { get; set; }
        public string AttachmentOutputName { get; set; }
        public string TimeStampSeconds { get; set; }
        public HighwayEmailStatusDto Status { get; set; }
        public DateTime AttachmentCreationDate { get; set; }
        
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }

        public Guid? ServiceId { get; set; }
        public ServiceDto Service { get; set; }

        //public byte[] File { get; set; }
        public string[] FileArray { get; set; }

        public DateTime CreationDate { get; set; }

        public Int64 Transaccion { get; set; }
        
        public string ClientIp { get; set; }

        public ICollection<HighwayEmailErrorDto> Errors { get; set; }
    }

}
