using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IAccountGroupCommandService
    {
        public async Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AddAccountGroupCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToAccountGroup())
                        .BindAsync(ValidateIfParentAccountGroupExistsAsync)
                        .BindAsync(ValidateIfClassificationExistsAsync)
                        .BindAsync(AddInDbContextAsync)
                        .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateAsync(UpdateAccountGroupCommand command)
        {
            var model = command.ToAccountGroup();

            return await GetAsync(model.Id)
                .BindAsync(ag => MapInDbContextAsync(ag, model))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(ValidateIfParentAccountGroupExistsAsync)
                .BindAsync(ValidateIfClassificationExistsAsync)
                .BindAsync(UpdateInDbContext)
                .BindAsync(UpdateSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> MapInDbContextAsync
            (AccountGroup current, AccountGroup forUpdate)
        {
            current = forUpdate.ToAccountGroup(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> AddSaveAndPublishAsync(AccountGroup accountGroup)
        {
            await publishEndpoint.Publish(accountGroup.ToAccountGroupAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateSaveAndPublishAsync(AccountGroup accountGroup)
        {
            try
            {
                await publishEndpoint.Publish(accountGroup.ToAccountGroupUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return accountGroup;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("AccountGroup", accountGroup.Version.ToString());
            }
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateInDbContext(AccountGroup accountGroup)
        {
            ctx.Entry(accountGroup).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfNotAlreadyExistsWithOtherIdAsync(AccountGroup accountGroup)
        {
            var exists = await ctx.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId
                        && a.Id != accountGroup.Id);

            return exists
                ? new ResourceAlreadyExistsError("AccountGroup_In_Classification",
                    "Code/ClassificationId/Id", $"{accountGroup.Code}/{accountGroup.ClassificationId}/{accountGroup.Id}")
                : accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await ctx.AccountGroups.FindAsync(id);

            return accountGroup == null
                ? new ResourceNotFoundError("AccountGroup", "Id", id.ToString())
                : accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> AddInDbContextAsync(AccountGroup accountGroup)
        {
            accountGroup.Id = NewId.NextGuid();
            await ctx.AccountGroups.AddAsync(accountGroup);
            ctx.SetAuditAndSpecialFields();

            return accountGroup;

        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfNotAlreadyExistsAsync(AccountGroup accountGroup)
        {
            var exists = await ctx.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId);

            return exists
                ? new ResourceAlreadyExistsError("AccountGroup_In_Classification",
                    "Code/ClassificationId", $"{accountGroup.Code}/{accountGroup.ClassificationId}")
                : accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfClassificationExistsAsync(AccountGroup accountGroup)
        {
            return await ctx.Classifications.AnyAsync(a => a.Id == accountGroup.ClassificationId)
                ? accountGroup
                : new BadParamExternalResourceNotFound("AccountGroup", "Classification", "ClassificationId", accountGroup.ClassificationId.ToString());
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfParentAccountGroupExistsAsync(AccountGroup accountGroup)
        {
            return accountGroup.ParentAccountGroupId != null
                ? await ctx.AccountGroups.AnyAsync(a => a.Id == (Guid)accountGroup.ParentAccountGroupId)
                    ? accountGroup
                    : new AccountGroupParentNotFoundError((Guid)accountGroup.ParentAccountGroupId)
                : accountGroup;
        }
    }
}
