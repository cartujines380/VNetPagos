using System;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class PageModel
    {
        public Guid Id { get; set; }

        public PageTypeDto PageType { get; set; }

        [AllowHtml]
        public string Content { get; set; }
    }
}
