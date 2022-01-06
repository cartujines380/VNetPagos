using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class FileDto
    {
        public byte[] ArrBytes { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}
