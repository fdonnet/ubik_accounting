using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands
{
    public record UpdateSalesOrVatTaxRateCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        public DateOnly ValidFrom { get; init; }
        public DateOnly? ValidTo { get; init; }
        [Required]
        [MaxLength(20)]
        public required string Code { get; init; }
        [MaxLength(200)]
        public string? Description { get; init; }
        [Required]
        public decimal Rate { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
