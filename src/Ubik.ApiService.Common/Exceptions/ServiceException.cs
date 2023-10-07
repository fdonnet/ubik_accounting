using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ubik.ApiService.Common.Exceptions
{
    public enum ServiceExceptionType
    {
        OK = 1,
        NotFound = 2,
        BadParams = 3,
        Conflict = 4,
        NotAuthorized = 5,
        NotAuthentified = 6
    }
    public class ServiceException : Exception
    {
        public ServiceExceptionType ExceptionType { get; set; } //Allow to identify what we need to do
        public required string ErrorFriendlyMessage { get; set; } //English message for internal purposes
        public required string ErrorCode { get; set; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public required string ErrorValueDetails { get; set; } //The value that raises the error
    }
}
