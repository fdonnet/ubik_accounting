﻿using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class Classification : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
