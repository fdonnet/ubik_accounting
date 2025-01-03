﻿using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Security.Api.Models
{
    public class RoleAuthorization : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid AuthorizationId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
