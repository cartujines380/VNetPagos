using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class HomePageItemDto
    {
        public Guid Id { get; set; }

        public string Headline1 { get; set; }

        public string Headline2 { get; set; }

        public string Description { get; set; }

        public Guid? ImageId { get; set; }
        public virtual ImageDto Image { get; set; }

        public string LinkUrl { get; set; }

        public Guid? FileId { get; set; }
        public virtual ImageDto File { get; set; }

        public string LinkName { get; set; }
    }
}
