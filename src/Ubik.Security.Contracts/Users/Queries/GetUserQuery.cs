using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Users.Queries
{
    public record GetUserQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
