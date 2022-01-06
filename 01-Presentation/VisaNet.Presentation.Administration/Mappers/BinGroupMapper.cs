using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class BinGroupMapper
    {
        public static BinGroupDto ToDto(this BinGroupModel entity)
        {
            var dto = new BinGroupDto
            {
                Id = entity.Id,
                Name = entity.Name,
                AddedBins = entity.AddedBins != null ? entity.AddedBins.Select(b => new BinDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Country = b.Country,
                    Value = int.Parse(b.Value)
                }).ToList() : new List<BinDto>(),
                DeletedBins = entity.DeletedBins != null ? entity.DeletedBins.Select(b => new BinDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Country = b.Country,
                    Value = int.Parse(b.Value)
                }).ToList() : new List<BinDto>(),                
                Services = entity.Services!=null ? entity.Services.Select( s => new ServiceDto { Id = s } ).ToList() : new List<ServiceDto>()
            };

            return dto;
        }

        public static BinGroupModel ToModel(this BinGroupDto entity)
        {
            
            var model = new BinGroupModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Bins = entity.Bins != null ? entity.Bins.Select(x => x.ToModel()).ToList() : null,
                Services = entity.Services != null ? entity.Services.Select(x => x.Id).ToList() : null,                
            };

            return model;
        }
    }
}