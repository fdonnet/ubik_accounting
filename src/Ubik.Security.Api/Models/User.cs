﻿using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Security.Api.Models
{
    public class User : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public bool IsActivated { get; set; } = true;
        public bool IsMegaAdmin { get; set; } = false;
        public Guid? SelectedTenantId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
