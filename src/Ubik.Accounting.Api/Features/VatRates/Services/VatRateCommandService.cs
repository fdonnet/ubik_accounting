using LanguageExt;
using MassTransit.Transports;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.VatRate.Commands;
using Ubik.Accounting.Contracts.VatRate.Events;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Mappers;

namespace Ubik.Accounting.Api.Features.VatRates.Services
{
    public class VatRateCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IVatRateCommandService
    {
        public async Task<Either<IServiceAndFeatureError, VatRate>> AddAsync(AddVatRateCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToVatRate())
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, VatRate>> UpdateAsync(UpdateVatRateCommand command)
        {
            var model = command.ToVatRate();

            return await GetAsync(model.Id)
                .BindAsync(x => MapInDbContextAsync(x, model))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(UpdateInDbContextAsync)
                .BindAsync(UpdateSaveAndPublishAsync);
        }

        //TODO: need to implement check if used in transactions and soft delete
        public async Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeletedSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, bool>> DeletedSaveAndPublishAsync(VatRate current)
        {
            await publishEndpoint.Publish(new VatRateDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return true;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> DeleteInDbContextAsync(VatRate current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> UpdateSaveAndPublishAsync(VatRate current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToVatRateUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();

                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("VatRate", current.Version.ToString());
            }
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> GetAsync(Guid id)
        {
            var result = await ctx.VatRates.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("VatRate", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> UpdateInDbContextAsync(VatRate current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> ValidateIfNotAlreadyExistsWithOtherIdAsync(VatRate current)
        {
            var exists = await ctx.VatRates.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("VatRate", "Code", current.Code)
                : current;
        }

        private static async Task<Either<IServiceAndFeatureError, VatRate>> MapInDbContextAsync
            (VatRate current, VatRate forUpdate)
        {
            current = forUpdate.ToVatRate(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> AddSaveAndPublishAsync(VatRate current)
        {
            await publishEndpoint.Publish(current.ToVatRateAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> AddInDbContextAsync(VatRate current)
        {
            current.Id = NewId.NextGuid();
            await ctx.VatRates.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, VatRate>> ValidateIfNotAlreadyExistsAsync(VatRate current)
        {
            var exists = await ctx.VatRates.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("VatRate", "Code", current.Code)
                : current;
        }
    }
}
