using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public  record GetAccountGroupClassificationResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public Guid ClassificationId { get; init; }
        public string ClassificationCode { get; init; } = default!;
        public string CLassificationLabel { get; init; } = default!;
    }
}
