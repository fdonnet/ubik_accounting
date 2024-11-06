using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Services
{
    public class TaxRateCommandService(AccountingSalesTaxDbContext ctx, IPublishEndpoint publishEndpoint) : ITaxRateCommandService
    {
        public async Task<Either<IFeatureError, TaxRate>> AddAsync(AddTaxRateCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToTaxRate())
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IFeatureError, TaxRate>> UpdateAsync(UpdateTaxRateCommand command)
        {
            var model = command.ToTaxRate();

            return await GetAsync(model.Id)
                .BindAsync(x => MapInDbContextAsync(x, model))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(UpdateInDbContextAsync)
                .BindAsync(UpdateSaveAndPublishAsync);
        }

        //TODO: need to implement check if used in transactions and soft delete
        public async Task<Either<IFeatureError, bool>> DeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeletedSaveAndPublishAsync);
        }

        private async Task<Either<IFeatureError, bool>> DeletedSaveAndPublishAsync(TaxRate current)
        {
            await publishEndpoint.Publish(new TaxRateDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return true;
        }

        private async Task<Either<IFeatureError, TaxRate>> DeleteInDbContextAsync(TaxRate current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, TaxRate>> UpdateSaveAndPublishAsync(TaxRate current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToTaxRateUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();

                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("TaxRate", current.Version.ToString());
            }
        }

        private async Task<Either<IFeatureError, TaxRate>> GetAsync(Guid id)
        {
            var result = await ctx.TaxRates.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("TaxRate", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IFeatureError, TaxRate>> UpdateInDbContextAsync(TaxRate current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, TaxRate>> ValidateIfNotAlreadyExistsWithOtherIdAsync(TaxRate current)
        {
            var exists = await ctx.TaxRates.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("TaxRate", "Code", current.Code)
                : current;
        }

        private static async Task<Either<IFeatureError, TaxRate>> MapInDbContextAsync
            (TaxRate current, TaxRate forUpdate)
        {
            current = forUpdate.ToTaxRate(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, TaxRate>> AddSaveAndPublishAsync(TaxRate current)
        {
            await publishEndpoint.Publish(current.ToTaxRateAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IFeatureError, TaxRate>> AddInDbContextAsync(TaxRate current)
        {
            current.Id = NewId.NextGuid();
            await ctx.TaxRates.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IFeatureError, TaxRate>> ValidateIfNotAlreadyExistsAsync(TaxRate current)
        {
            var exists = await ctx.TaxRates.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("TaxRate", "Code", current.Code)
                : current;
        }
    }
}
