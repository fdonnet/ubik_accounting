using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Users.Commands
{
    public record AddUserCommand
    {
        [Required]
        [MaxLength(100)]
        public string Firstname { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Lastname { get; init; } = default!;
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; init; } = default !;
        [Required]
        [MaxLength(60)]
        //TODO: change min len and go with other stuff for auth
        [MinLength(4)]
        public string Password { get; init; } = default!;
    }
}


