using Microsoft.EntityFrameworkCore;
using System.Data;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountUpdateDbConcurrencyException : DbUpdateConcurrencyException, IServiceAndFeatureException
    {
        public required string ErrorFriendlyMessage { get; set; } //English message for internal purposes
        public required string ErrorCode { get; set; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public required string ErrorValueDetails { get; set; } //The value that raises the error

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbUpdateConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public AccountUpdateDbConcurrencyException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
