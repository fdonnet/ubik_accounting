﻿using Ubik.DB.Common;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class AccountAccountGroup : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid AccountGroupId { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
