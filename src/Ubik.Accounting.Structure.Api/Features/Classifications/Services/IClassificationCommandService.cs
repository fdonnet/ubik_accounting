using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.Services
{
    public interface IClassificationCommandService
    {
        public Task<Either<IFeatureError, Classification>> AddAsync(AddClassificationCommand command);
        public Task<Either<IFeatureError, Classification>> UpdateAsync(UpdateClassificationCommand command);
        public Task<Either<IFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
