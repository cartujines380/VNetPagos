using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
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