using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace VisaNet.Utilities.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RegexValidation : ValidationAttribute, IClientValidatable
    {
        private string RegexDefinitionProperty { get; set; }

        public RegexValidation(string regexDefinitionProperty)
        {
            RegexDefinitionProperty = regexDefinitionProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            var regexPatern = (string)model.GetType().GetProperties().First(prop => prop.Name == RegexDefinitionProperty).GetValue(model);

            if (string.IsNullOrWhiteSpace(regexPatern) || value == null)
                return ValidationResult.Success;
            
            var regex = new Regex(regexPatern);
            var valueAsString = (string)value;

            return regex.IsMatch(valueAsString) ? ValidationResult.Success : new ValidationResult("El formato no es válido");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var parentType = metadata.ContainerType;
            var model = context.Controller.ViewData.Model;

            //If the type of the view model doesn't match the type of the object, it has to search on properties and find the right element
            if (parentType != context.Controller.ViewData.Model.GetType())
            {
                model = model.GetType().GetProperties().First(prop => prop.PropertyType == parentType).GetValue(model);
            }
            var parentMetaData = ModelMetadataProviders.Current.GetMetadataForProperties(model, parentType);
            var regexPatern = (string)parentMetaData.FirstOrDefault(p => p.PropertyName == RegexDefinitionProperty).Model;

            const string errorMessage = "El formato no es válido";

            // The value we set here are needed by the jQuery adapter
            var dateGreaterThanRule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = "regexvalidation"
            };
            //"otherpropertyname" is the name of the jQuery parameter for the adapter, must be LOWERCASE!
            dateGreaterThanRule.ValidationParameters.Add("regexvalidation", regexPatern);

            yield return dateGreaterThanRule;
        }
    }
}