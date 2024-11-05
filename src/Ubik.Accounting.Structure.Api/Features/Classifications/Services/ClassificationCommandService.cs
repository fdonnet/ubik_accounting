using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.Accounting.Structure.Contracts.Classifications.Events;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.Services
{
    public class ClassificationCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IClassificationCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Classification>> AddAsync(AddClassificationCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToClassification())
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> UpdateAsync(UpdateClassificationCommand command)
        {
            var model = command.ToClassification();

            return await GetAsync(model.Id)
                .BindAsync(c => MapInDbContextAsync(c,model))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(UpdateInDbContextAsync)
                .BindAsync(UpdateSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id)
        {
            using var transaction = ctx.Database.BeginTransaction();
            var deletedAccountGroups = new List<AccountGroup>();

            return await GetAsync(id)
                .BindAsync(c=> GetFirstLvlAccountGroupsAsync(id))
                .BindAsync(ag => DeleteFromParentGroupsAsync(ag, deletedAccountGroups))
                .BindAsync(del => DeleteClassificationAsync(id, del))
                .BindAsync(del => DeleteSaveAndPublishAsync(id, del, transaction));
        }

        private async Task<Either<IServiceAndFeatureError,List<AccountGroup>>> DeleteSaveAndPublishAsync(Guid id, List<AccountGroup> ag, IDbContextTransaction trans)
        {
            await publishEndpoint.Publish(new ClassificationDeleted()
            {
                Id = id,
                AccountGroupsDeleted = ag.ToAccountGroupStandardResults()

            }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            trans.Commit();

            return ag;
        }

        private async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteClassificationAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            await ctx.Classifications.Where(x => x.Id == id).ExecuteDeleteAsync();
            return deletedAccountGroups;
        }

        private async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteFromParentGroupsAsync(List<AccountGroup> firstLvlAccountGroups, List<AccountGroup> deletedAccountGroups)
        {
            foreach (var ag in firstLvlAccountGroups)
            {
                deletedAccountGroups.Add(ag);
                await DeleteAllChildrenAccountGroupsAsync(ag.Id, deletedAccountGroups);
                await ctx.AccountGroups.Where(x => x.Id == ag.Id).ExecuteDeleteAsync();
            }

            return deletedAccountGroups;
        }

        private async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAllChildrenAccountGroupsAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            var children = await ctx.AccountGroups.Where(ag => ag.ParentAccountGroupId == id).ToListAsync();

            foreach (var child in children)
            {
                await DeleteAllChildrenAccountGroupsAsync(child.Id, deletedAccountGroups);
                deletedAccountGroups.Add(child);
                await ctx.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }

            return deletedAccountGroups;
        }

        private async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> GetFirstLvlAccountGroupsAsync(Guid classificationId)
        {
            return await ctx.AccountGroups
                                    .Where(ag => ag.ClassificationId == classificationId
                                                && ag.ParentAccountGroupId == null).ToListAsync();
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> UpdateSaveAndPublishAsync(Classification current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToClassificationUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();

                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("Classification", current.Version.ToString());
            }
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> UpdateInDbContextAsync(Classification current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private static async Task<Either<IServiceAndFeatureError, Classification>> MapInDbContextAsync
            (Classification current, Classification forUpdate)
        {
            current = forUpdate.ToClassification(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Classification classification)
        {
            var exists = await ctx.Classifications.AnyAsync(a => a.Code == classification.Code && a.Id != classification.Id);

            return exists
                ? new ResourceAlreadyExistsError("Classification", "Code", classification.Code)
                : classification;
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id)
        {
            var result = await ctx.Classifications.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Classification", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> AddSaveAndPublishAsync(Classification current)
        {
            await publishEndpoint.Publish(current.ToClassificationAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> AddInDbContextAsync(Classification current)
        {
            current.Id = NewId.NextGuid();
            await ctx.Classifications.AddAsync(current);
            ctx.SetAuditAndSpecialFields();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsAsync(Classification classification)
        {
            var exists = await ctx.Classifications.AnyAsync(a => a.Code == classification.Code);

            return exists
                ? new ResourceAlreadyExistsError("Classification", "Code", classification.Code)
                : classification;
        }
    }
}
