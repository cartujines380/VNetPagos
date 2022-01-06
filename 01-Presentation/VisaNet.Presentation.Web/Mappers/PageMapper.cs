using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class PageMapper
    {
        public static PageDto ToDto(this PageModel entity)
        {
            return new PageDto
            {
                Id = entity.Id,
                PageType = entity.PageType,
                Content = entity.Content
            };
        }

        public static PageModel ToModel(this PageDto entity)
        {
            return new PageModel
            {
                Id = entity.Id,
                PageType = entity.PageType,
                Content = entity.Content
            };
        }
    }
}