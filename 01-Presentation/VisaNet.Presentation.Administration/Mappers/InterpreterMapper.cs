using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class InterpreterMapper
    {
        public static InterpreterDto ToDto(this InterpreterModel entity)
        {
            var dto = new InterpreterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
            };

            if (entity.Services != null && entity.Services.Any())
            {
                dto.Services = entity.Services.Select(x => new ServiceDto()
                {
                    Active = x.Active,
                    Name = x.Name,
                    Id = x.Id,
                }).ToList();
            }

            return dto;
        }

        public static InterpreterModel ToModel(this InterpreterDto entity)
        {
            var model = new InterpreterModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                FileName = entity.Id + ".dll"
            };
            if (entity.Services != null && entity.Services.Any())
            {
                model.Services = entity.Services.Select(x => new ServiceModel()
                {
                    Active = x.Active,
                    Name = x.Name,
                    Id = x.Id,
                }).ToList();
            }


            return model;
        }
    }
}