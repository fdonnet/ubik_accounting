using Ubik.DB.Common.Models;

namespace Ubik.DB.Common
{
    public interface IAuditEntity
    {
        public AuditData AuditInfo { get; set; }
    }
}
