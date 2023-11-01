﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationsResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Label { get; set; } = default!;
        public string? Description { get; set; }
        public Guid Version { get; set; }
    }
}
