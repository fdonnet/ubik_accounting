using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    public class UpdateDbConcurrencyException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UpdateDbConcurrencyException()
        {
            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "DB_CONCURRENCY_CONFLICT",
                ErrorFriendlyMessage = "You don't have the last version of this ressource, refresh your data before updating.",
                ErrorValueDetails = $"Field:Version",
            }};
        }
    }
}
