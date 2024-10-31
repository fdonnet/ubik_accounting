using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.Services
{
    public interface IClassificationCommandService
    {
        public Task<Either<IServiceAndFeatureError, Classification>> AddAsync(AddClassificationCommand command);
        public Task<Either<IServiceAndFeatureError, Classification>> UpdateAsync(UpdateClassificationCommand command);
        public Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
