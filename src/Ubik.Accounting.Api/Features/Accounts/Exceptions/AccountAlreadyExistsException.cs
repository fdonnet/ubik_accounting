﻿using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public record AccountAlreadyExistsException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; set; }
        public List<CustomError> CustomErrors { get; set; }

        public AccountAlreadyExistsException(string codeAlreadyExisting)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{codeAlreadyExisting}"
            }};
        }
    }

    public record AccountAlreadyExistsException2 : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; set; } = ServiceAndFeatureExceptionType.BadParams;
        public List<CustomError> CustomErrors { get; set; } = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:"
            }};

    }
}
