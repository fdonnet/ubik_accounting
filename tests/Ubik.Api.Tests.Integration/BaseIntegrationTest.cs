﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data;

namespace Ubik.Api.Tests.Integration
{
    [Collection("Proxy")]
    public abstract class BaseIntegrationTest : IDisposable
    {
        private readonly IServiceScope _scope;
        internal IntegrationTestProxyFactory Factory { get; }


        internal BaseIntegrationTest(IntegrationTestProxyFactory factory)
        {
            Factory = factory;
            _scope = factory.Services.CreateScope();
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _scope?.Dispose();
        }
    }
}
