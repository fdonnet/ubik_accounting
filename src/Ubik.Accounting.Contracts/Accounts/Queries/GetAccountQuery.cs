using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Queries
{
    public record GetAccountQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
