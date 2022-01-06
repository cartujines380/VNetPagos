using VisaNet.Common.Resource.Models;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class Tc33Mapper
    {
        public static Tc33Model ToModel(this Tc33Dto entity)
        {
            return new Tc33Model
            {
                Id = entity.Id,
                CreationDate = entity.CreationDate.ToString("G"),
                InputFileName = entity.InputFileName,
                OutputFileName = entity.OutputFileName,
                //State = EnumHelpers.GetName(typeof(Tc33StateDto), (int)entity.State, ModelsStrings.ResourceManager),
                State = ModelsStrings.ResourceManager.GetString(entity.State.ToString()),
                StateValue = (int)entity.State,
                CreationUser = entity.CreationUser,
                TransactionNotFound = entity.Errors
            };
        }
    }
}