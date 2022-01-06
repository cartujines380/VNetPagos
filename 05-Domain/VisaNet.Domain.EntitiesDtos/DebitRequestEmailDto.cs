using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DebitRequestEmailDto
    {
        public string ProductName { get; set; }
        public string ServiceName  { get; set; }
        public string Status  { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> References  { get; set; }
        public string Email  { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string MaskedNumber  { get; set; }
    }
}
