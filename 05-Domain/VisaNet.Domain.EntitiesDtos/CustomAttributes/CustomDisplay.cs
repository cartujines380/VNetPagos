using System.ComponentModel;
using VisaNet.Common.Resource.EntitiesDto;

namespace VisaNet.Domain.EntitiesDtos.CustomAttributes
{
    public class CustomDisplay : DisplayNameAttribute
    {
        private readonly string _text;
        private readonly string _name;

        public CustomDisplay(string name)
        {
            _name = name;
            _text = EntitiesDtosStrings.ResourceManager.GetString(name);
        }

        public override string DisplayName
        {
            get { return _text ?? _name; }
        }
    }
}
