using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    public class CustomValidationProblemDetails : ValidationProblemDetails
    {
        public CustomValidationProblemDetails(IEnumerable<ProblemDetailErrors> errors)
        {
            Errors = errors;
        }

        [JsonPropertyName("errors")]
        public new IEnumerable<ProblemDetailErrors> Errors { get; } = new List<ProblemDetailErrors>();
        public CustomValidationProblemDetails(ModelStateDictionary modelState)
        {
            Errors = ConvertModelStateErrorsToErrors(modelState);
        }

        private List<ProblemDetailErrors> ConvertModelStateErrorsToErrors(ModelStateDictionary modelStateDictionary)
        {
            List<ProblemDetailErrors> validationErrors = new();
            foreach (var keyModelStatePair in modelStateDictionary)
            {
                var errors = keyModelStatePair.Value.Errors;

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        validationErrors.Add(new ProblemDetailErrors()
                        {
                            Code = "VALIDATION_ERROR",
                            FriendlyMsg = "Validation error occured with the data you submited, pls correct your payload.",
                            ValueInError = error.ErrorMessage
                        });
                    }
                }
            }
            return validationErrors;
        }
    }
}
