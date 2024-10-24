﻿namespace Ubik.ApiService.Common.Errors
{
    public record ResourceAlreadyExistsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceAlreadyExistsError(string resourceName, string fieldName, string value)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName.ToUpper()}_ALREADY_EXISTS",
                ErrorFriendlyMessage = $"The {resourceName} already exists. {fieldName} field needs to be unique.",
                ErrorValueDetails = $"Field:{fieldName} / Value:{value}"
            }];
        }
    }
}
