using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Events;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Services
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

        public async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id)
        {
            using var transaction = ctx.Database.BeginTransaction();
            var deletedAccountGroups = new List<AccountGroup>();

            return await GetAsync(id)
                .BindAsync(ag => DeleteAllChildrenOfAsync(ag, deletedAccountGroups))
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(ag => DeleteSaveCommitAndPublishAsync(ag, deletedAccountGroups, transaction));
        }

        private async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteSaveCommitAndPublishAsync(AccountGroup current,
            List<AccountGroup> childAccountGroups, IDbContextTransaction trans)
        {
            childAccountGroups.Add(current);
            await publishEndpoint.Publish(new AccountGroupsDeleted {  AccountGroups = childAccountGroups.ToAccountGroupDeleted() }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            await trans.CommitAsync();
            return childAccountGroups;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> DeleteAllChildrenOfAsync(AccountGroup current, List<AccountGroup> deletedAccountGroups)
        {
            var children = await ctx.AccountGroups.Where(ag => ag.ParentAccountGroupId == current.Id).ToListAsync();

            foreach (var child in children)
            {
                await DeleteAllChildrenOfAsync(child, deletedAccountGroups);
                deletedAccountGroups.Add(child);
                await ctx.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> DeleteInDbContextAsync(AccountGroup current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
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
