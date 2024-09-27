using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Errors
{
    public record ResourceNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceNotFoundError(string resourceName, string fieldName, string value)
        {
            ErrorType = ServiceAndFeatureErrorType.NotFound;
            CustomErrors = [ new CustomError()
            {
                ErrorCode = $"{resourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {resourceName} doesn't exist. {fieldName} not found.",
                ErrorValueDetails = $"Field:{fieldName} / Value:{value}",
            }];
        }
    }
}
