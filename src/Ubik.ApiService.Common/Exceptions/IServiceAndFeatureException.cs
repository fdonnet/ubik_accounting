using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ubik.ApiService.Common.Exceptions
{
    public enum ServiceAndFeatureExceptionType
    {
        OK = 1,
        NotFound = 2,
        BadParams = 3,
        Conflict = 4,
        NotAuthorized = 5,
        NotAuthentified = 6
    }

    //TODO: TO BE REMOVED, only keep the interface
    public class ServiceAndFeatureException : Exception
    {
        public ServiceAndFeatureExceptionType ExceptionType { get; set; } //Allow to identify what we need to do
        public required string ErrorFriendlyMessage { get; set; } //English message for internal purposes
        public required string ErrorCode { get; set; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public required string ErrorValueDetails { get; set; } //The value that raises the error
    }

    public interface IServiceAndFeatureException
    {
        public string ErrorFriendlyMessage { get; set; } //English message for internal purposes
        public string ErrorCode { get; set; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public string ErrorValueDetails { get; set; } //The value that raises the error
    }
}
