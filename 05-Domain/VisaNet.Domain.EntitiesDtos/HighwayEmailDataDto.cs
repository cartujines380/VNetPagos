using System.Collections.Generic;
using System.Net.Mail;

namespace VisaNet.Domain.EntitiesDtos
{
    public class HighwayEmailDataDto
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public IEnumerable<HighwayEmailErrorDto> Errors { get; set; }
        public string CodCommerce { get; set; }
        public string CodBranch { get; set; }
        public string FileName { get; set; }
        public string TransactionNumber { get; set; }
        public string ServiceName { get; set; }
        public string RejectedMessage { get; set; }
        public int Count { get; set; }
        public int CountN { get; set; }
        public int CountD { get; set; }
        public double ValueN { get; set; }
        public double ValueD { get; set; }
        public string Message { get; set; }

        public Attachment AttachmentFile { get; set; }

        public string FilePath { get; set; }
        public string MimeType { get; set; }
    
    }
}
