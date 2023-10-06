namespace Ubik.DB.Common
{
    public interface IAuditEntity
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }

    }
}
