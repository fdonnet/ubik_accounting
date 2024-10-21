using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationCommandService(AccountingDbContext ctx) : IClassificationCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Classification>> AddAsync(AddClassificationCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToClassification())
                .BindAsync(AddSaveInDbContextAsync);
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
