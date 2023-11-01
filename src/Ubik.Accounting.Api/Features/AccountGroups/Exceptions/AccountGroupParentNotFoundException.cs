﻿using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public record AccountGroupParentNotFoundException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupParentNotFoundException(Guid parentAccountGroupId)
        {

            ErrorType = ServiceAndFeatureExceptionType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "PARENT_ACCOUNTGROUP_NOTFOUND",
                ErrorFriendlyMessage = "The parent account group specified is not found.",
                ErrorValueDetails = $"Field:ParentAccountGroupId / Value:{parentAccountGroupId}"
            }};
        }
    }
}
