using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VisaNet.Utilities.Helpers
{
    public class DataValidation
    {

        /// <summary>
        /// Executes validations against de 'obj' parameter.
        /// </summary>
        /// <param name="obj">The model to validate</param>
        /// <returns>The errors generated from the validation</returns>
        private static IList<ValidationResult> GetValidationErrors(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(obj, null, null);
            Validator.TryValidateObject(obj, context, validationResults, true);
            return validationResults;
        }

        /// <summary>
        /// Checks that the input parameters of the webmethod are correct
        /// </summary>
        /// <param name="inputParameters">The model containing the input parameters. The model's properties should be marked with DataAnnotations</param>
        /// <param name="errorMsg">Returns a message with the details of the errors</param>
        /// <returns>True in case all parameters are correct. False otherwise.</returns>
        public static bool InputParametersAreValid(object inputParameters, out string errorMsg)
        {
            //Validate object and recieve errors
            IList<ValidationResult> errors = GetValidationErrors(inputParameters);

            //If there are no errors, return true and no error message
            if (!errors.Any())
            {
                errorMsg = string.Empty;
                return true;
            }

            //In case of errors
            errorMsg = "Se generaron los siguientes errores: ";
            int i = 1;
            foreach (var validationResult in errors)
            {
                errorMsg += validationResult.ErrorMessage + (i == errors.Count ? "":  " - ");
                i++;
            }

            return false;
        } 
    }
}