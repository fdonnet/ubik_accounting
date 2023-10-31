using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record AddAccountGroupResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid? ParentAccountGroupId { get; init; }
        public Guid AccountGroupClassificationId { get; init; }
        public Guid Version { get; init; }
    }
}
