﻿namespace Ubik.ApiService.Common.Errors
{
    public record ResourceUpdateConcurrencyError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceUpdateConcurrencyError(string resourceName, string version)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = $"{resourceName.ToUpper()}_UPDATE_CONCURRENCY",
                ErrorFriendlyMessage = $"You don't have the last version of this {resourceName}.",
                ErrorValueDetails = $"Field:Version / Value:{version}"
            }};
        }
    }
}