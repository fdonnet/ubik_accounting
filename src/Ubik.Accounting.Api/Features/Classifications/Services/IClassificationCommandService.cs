using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationCommandService
    {
        public Task<Either<IServiceAndFeatureError, Classification>> AddAsync(AddClassificationCommand command);
        public Task<Either<IServiceAndFeatureError, Classification>> UpdateAsync(UpdateClassificationCommand command);
        public Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
