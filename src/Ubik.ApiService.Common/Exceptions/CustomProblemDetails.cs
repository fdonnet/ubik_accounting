using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Ubik.ApiService.Common.Exceptions
{
    public class CustomProblemDetails : ValidationProblemDetails
    {
        public CustomProblemDetails(IEnumerable<ProblemDetailError> errors)
        {
            Errors = errors;
        }

        [JsonPropertyName("errors")]
        public new IEnumerable<ProblemDetailError> Errors { get; } = new List<ProblemDetailError>();
        public CustomProblemDetails(ModelStateDictionary modelState)
        {
            Errors = ConvertModelStateErrorsToErrors(modelState);
        }

        private static List<ProblemDetailError> ConvertModelStateErrorsToErrors(ModelStateDictionary modelStateDictionary)
        {
            List<ProblemDetailError> validationErrors = new();
            foreach (var keyModelStatePair in modelStateDictionary)
            {
                var errors = keyModelStatePair.Value.Errors;

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        validationErrors.Add(new ProblemDetailError()
                        {
                            Code = "VALIDATION_ERROR",
                            FriendlyMsg = error.ErrorMessage,
                            ValueInError = keyModelStatePair.Key
                        });
                    }
                }
            }
            return validationErrors;
        }
    }
}
