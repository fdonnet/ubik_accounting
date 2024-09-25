using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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


