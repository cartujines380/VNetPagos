using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Filters
{
    public class DataMinMax : ValidationAttribute, IClientValidatable
    {
        private readonly string _minPPropertyName;
        private readonly string _maxPPropertyName;
        private readonly string _minDPropertyName;
        private readonly string _maxDPropertyName;
        private readonly string _currencyName;

        public DataMinMax(string minP, string maxP, string minD, string maxD, string currency)
        {
            _minPPropertyName = minP;
            _maxPPropertyName = maxP;
            _minDPropertyName = minD;
            _maxDPropertyName = maxD;
            _currencyName = currency;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minPProperty = validationContext.ObjectType.GetProperty(_minPPropertyName);
            var maxPProperty = validationContext.ObjectType.GetProperty(_maxPPropertyName);
            var minDProperty = validationContext.ObjectType.GetProperty(_minDPropertyName);
            var maxDProperty = validationContext.ObjectType.GetProperty(_maxDPropertyName);
            var currencyProperty = validationContext.ObjectType.GetProperty(_currencyName);

            if (minPProperty == null)
                return new ValidationResult(string.Format("Unknown property {0}", _minPPropertyName));
            if (maxPProperty == null)
                return new ValidationResult(string.Format("Unknown property {0}", _maxPPropertyName));
            if (minDProperty == null)
                return new ValidationResult(string.Format("Unknown property {0}", _minDPropertyName));
            if (maxDProperty == null)
                return new ValidationResult(string.Format("Unknown property {0}", _maxDPropertyName));
            if (currencyProperty == null)
                return new ValidationResult(string.Format("Unknown property {0}", _currencyName));

            var minPValue = (int)minPProperty.GetValue(validationContext.ObjectInstance, null);
            var maxPValue = (int)maxPProperty.GetValue(validationContext.ObjectInstance, null);
            var minDValue = (int)minDProperty.GetValue(validationContext.ObjectInstance, null);
            var maxDValue = (int)maxDProperty.GetValue(validationContext.ObjectInstance, null);
            var currencyValue = (int)currencyProperty.GetValue(validationContext.ObjectInstance, null);

            var currentValue = (value == null ) ? 0 : (double) value;

            if (currencyValue == (int)CurrencyDto.UYU)
            {
                if (currentValue < minPValue || currentValue > maxPValue)
                {
                    return new ValidationResult(string.Format(ErrorMessage, minPValue, maxPValue));
                }    
            }
            if (currencyValue == (int)CurrencyDto.USD)
            {
                if (currentValue < minDValue || currentValue > maxDValue)
                {
                    return new ValidationResult(string.Format(ErrorMessage, minDValue, maxDValue));
                }
            }
            
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                       {
                           ValidationType = "dynamicrange",
                           ErrorMessage = ErrorMessage
                       };

            rule.ValidationParameters["minvalueproperty"] = _minPPropertyName;
            rule.ValidationParameters["maxvalueproperty"] = _maxPPropertyName;
            
            yield return rule;
        }
    }
}