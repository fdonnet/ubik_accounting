﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    //TODO: will be updated by another service
    [Index(nameof(TenantId), IsUnique = false)]
    [Table("Currencies")]
    public class Currency : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        [StringLength(3)]
        public required string IsoCode { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
