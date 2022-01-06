using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF
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
        public static bool AreInputParametersValid(object inputParameters, out string errorMsg)
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
            errorMsg = "Los siguientes parámetros faltan o son incorrectos: ";
            int i = 1;
            foreach (var validationResult in errors)
            {
                errorMsg += validationResult.ErrorMessage + (i == errors.Count ? "" : " - ");
                i++;
            }

            return false;
        }

        public static PreprocessPaymentResponse ValidatePreprocessPaymentInput(PreprocessPaymentData data)
        {
            //Chequeo que haya alguna factura
            if (!data.Bills.Any())
            {
                return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.NO_SE_INGRESO_NINGUNA_FACTURA_A_PAGAR, EnumsStrings.ResourceManager),
                        ResponseType = (int)ErrorCodeDto.NO_SE_INGRESO_NINGUNA_FACTURA_A_PAGAR
                    }
                };
            }

            //Chequeo que el servicio sea uno solo
            if (data.Bills.Any(b => b.ServiceId != data.Bills.FirstOrDefault().ServiceId))
            {
                return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_SER_DEL_MISMO_SERVICIO, EnumsStrings.ResourceManager),
                        ResponseType = (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_SER_DEL_MISMO_SERVICIO
                    }
                };
            }

            //Chequeo que la pasarela sea una sola
            var gateway = data.Bills.FirstOrDefault().Gateway;
            if (data.Bills.Any(b => b.Gateway != gateway))
            {
                return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_SER_DE_LA_MISMA_PASARELA, EnumsStrings.ResourceManager),
                        ResponseType = (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_SER_DE_LA_MISMA_PASARELA
                    }
                };
            }

            //Chequeo que se haya ingresado el bin
            if (data.Bills.FirstOrDefault().CardBinNumbers == default(int))
            {
                return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = String.Format("{0}: No se ingresó el BIN de la tarjeta.", EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERRORES_EN_LOS_CAMPOS_ENVIADOS, EnumsStrings.ResourceManager)),
                        ResponseType = (int)ErrorCodeDto.ERRORES_EN_LOS_CAMPOS_ENVIADOS
                    }
                };
            }

            //Chequeo que el bin sea uno solo
            if (data.Bills.Any(b => b.CardBinNumbers != data.Bills.FirstOrDefault().CardBinNumbers))
            {
                return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_TENER_EL_MISMO_BIN, EnumsStrings.ResourceManager),
                        ResponseType = (int)ErrorCodeDto.LAS_FACTURAS_A_PAGAR_DEBEN_TENER_EL_MISMO_BIN
                    }
                };
            }

            return new PreprocessPaymentResponse(VisaNetAccessResponseCode.Ok);
        }
    }
}