﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public interface IAddAccountResult
    {
       public bool IsSuccess { get; set; }
       public AddAccountResult? AddAccountResult { get; set; }
       public CustomProblemDetails? Fault { get; set; }
    }

    public record AddAccountResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Label { get; set; } = default!;
        public AccountCategory Category { get; set; }
        public AccountDomain Domain { get; set; }
        public string? Description { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid Version { get; set; }
    }
}