using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IClassificationCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Classification>> AddAsync(AddClassificationCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToClassification())
                .BindAsync(AddSaveInDbContextAsync)
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

        private async Task<Either<IServiceAndFeatureError, Classification>> AddSaveInDbContextAsync(Classification current)
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
