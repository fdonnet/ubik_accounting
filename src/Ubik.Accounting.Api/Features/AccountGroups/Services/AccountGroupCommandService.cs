using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IAccountGroupCommandService
    {
        public async Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AddAccountGroupCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToAccountGroup())
                        .BindAsync(ag => ValidateIfParentAccountGroupExistsAsync(ag))
                        .BindAsync(ag => ValidateIfClassificationExistsAsync(ag))
                        .BindAsync(ag => AddInDbContextAsync(ag))
                        .MapAsync(async ag =>
                        {
                            await publishEndpoint.Publish(ag.ToAccountGroupAdded(), CancellationToken.None);
                            await ctx.SaveChangesAsync();
                            return ag;
                        });
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
