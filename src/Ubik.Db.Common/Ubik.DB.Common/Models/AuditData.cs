namespace Ubik.DB.Common.Models
{
    public class AuditData
    {
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime ModifiedAt { get; private set; }
        public Guid ModifiedBy { get; private set; }

        public AuditData(DateTime createdAt, Guid createdBy, DateTime modifiedAt, Guid modifiedBy)
        {
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            ModifiedAt = modifiedAt;
            ModifiedBy = modifiedBy;
        }

        public void SetModified(DateTime modifiedAt, Guid modifiedBy)
        {
            ModifiedAt = modifiedAt;
            ModifiedBy = modifiedBy;
        }
    }
}
