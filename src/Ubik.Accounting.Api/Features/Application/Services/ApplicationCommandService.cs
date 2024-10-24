﻿using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Data.Init;

namespace Ubik.Accounting.Api.Features.Application.Services
{
    public class ApplicationCommandService(AccountingDbContext ctx, IWebHostEnvironment env) : IApplicationCommandService
    {
        public async Task<bool> CleanupDatabaseInDevAsync()
        {
            try
            {
                if (env.IsDevelopment())
                {
                    await ctx.Database.EnsureDeletedAsync();
                    await ctx.Database.EnsureCreatedAsync();
                    await DbInitializer.InitializeAsync(ctx);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
