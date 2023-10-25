﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts
{
    public record AccountUpdated
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public AccountCategory Category { get; set; }
        public AccountDomain Domain { get; set; }
        public string? Description { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}