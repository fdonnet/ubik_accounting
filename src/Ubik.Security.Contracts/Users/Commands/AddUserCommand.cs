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
        public string Email { get; init; } = default !;
    }
}


