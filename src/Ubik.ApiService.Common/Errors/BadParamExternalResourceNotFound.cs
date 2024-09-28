using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Errors
{
    public record BadParamExternalResourceNotFound : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public BadParamExternalResourceNotFound(string resourceName, string externalResourceName, string fieldName, string value)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = [ new CustomError()
            {
                ErrorCode = $"{resourceName.ToUpper()}_{externalResourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {externalResourceName} doesn't exist. Param {fieldName} not found.",
                ErrorValueDetails = $"Field:{fieldName} / Value:{value}",
            }];
        }
    }
}
