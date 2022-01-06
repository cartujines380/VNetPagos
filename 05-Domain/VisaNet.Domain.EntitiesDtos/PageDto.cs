using System;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class PageDto
    {
        public Guid Id { get; set; }

        public PageTypeDto PageType { get; set; }

        [AllowHtml]
        public string Content { get; set; }
    }
}
