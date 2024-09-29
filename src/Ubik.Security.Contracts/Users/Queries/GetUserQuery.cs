using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Users.Queries
{
    public record GetUserQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
