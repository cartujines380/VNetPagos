using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class DebitRequestReferenceMapper
    {
        public static DebitRequestReferenceModel ToModel(this DebitRequestReferenceDto entity)
        {
            var model = new DebitRequestReferenceModel
            {
                Id = entity.Id,
                Index = entity.Index,
                Value = entity.Value,
                ProductPropertyId = entity.ProductPropertyId,
            };

            return model;
        }
    }
}