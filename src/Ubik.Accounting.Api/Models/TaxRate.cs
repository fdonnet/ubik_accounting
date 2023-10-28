namespace Ubik.Accounting.Api.Models
{
    //[Index(nameof(TenantId), IsUnique = false)]
    //[Table("TaxRates")]
    //public class TaxRate : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    //{
    //    public Guid Id { get; set; }
    //    public DateTime ValidFrom { get; set; }
    //    public DateTime? ValidTo { get; set; }
    //    [StringLength(50)]
    //    public required string Name { get; set; }
    //    [StringLength(300)]
    //    public string? Description { get; set; }
    //    [Precision(10, 7)]
    //    public decimal Rate { get; set; }
    //    [ConcurrencyCheck]
    //    public Guid Version { get; set; }
    //    public Guid TenantId { get; set; }
    //    public required DateTime CreatedAt { get; set; }
    //    public required Guid CreatedBy { get; set; }
    //    public User CreatedByUser { get; set; } = default!;
    //    public DateTime? ModifiedAt { get; set; }
    //    public Guid? ModifiedBy { get; set; }
    //    public User? ModifiedByUser { get; set; }
    //}
}
