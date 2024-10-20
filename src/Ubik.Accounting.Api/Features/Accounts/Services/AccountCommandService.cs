using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public class AccountCommandService(AccountingDbContext ctx, ICurrentUser currentUser, IPublishEndpoint publishEndpoint) : IAccountCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Account>> AddAsync(Account account)
        {
            return await ValidateIfNotAlreadyExistsAsync(account)
                .BindAsync(ValidateIfCurrencyExistsAsync)
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, Account>> AddSaveAndPublishAsync(Account account)
        {
            await publishEndpoint.Publish(account.ToAccountAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> AddInDbContextAsync(Account account)
        {
            account.Id = NewId.NextGuid();
            await ctx.Accounts.AddAsync(account);
            ctx.SetAuditAndSpecialFields();
            return account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotAlreadyExistsAsync(Account account)
        {
            var exists = await ctx.Accounts.AnyAsync(a => a.Code == account.Code);
            return exists
                ? new ResourceAlreadyExistsError("Account", "Code", account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfCurrencyExistsAsync(Account account)
        {
            return await ctx.Currencies.AnyAsync(c => c.Id == account.CurrencyId)
                ? account
                : new BadParamExternalResourceNotFound("Account", "Currency", "CurrencyId", account.CurrencyId.ToString());
        }
    }
}
