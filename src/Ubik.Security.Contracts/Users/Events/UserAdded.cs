using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Users.Events
{
    public record UserAdded
    {
        public Guid Id { get; init; }
        public string Firstname { get; init; } = default!;
        public string Lastname { get; init; } = default!;
        public string Email { get; init; } = default!;
        public Guid Version { get; init; }
    }
}
