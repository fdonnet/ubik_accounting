﻿namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Label { get; set; } = default!;
        public string? Description { get; set; }
        public Guid Version { get; set; }
    }
}
