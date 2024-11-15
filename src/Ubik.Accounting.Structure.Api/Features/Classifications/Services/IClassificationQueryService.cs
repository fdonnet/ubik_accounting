using LanguageExt;
using Ubik.Accounting.Structure.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.Services
{
    public interface IClassificationQueryService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<Either<IFeatureError, Classification>> GetAsync(Guid id);
        public Task<Either<IFeatureError, IEnumerable<Account>>> GetClassificationAttachedAccountsAsync(Guid id);
        public Task<Either<IFeatureError, IEnumerable<Account>>> GetClassificationMissingAccountsAsync(Guid id);
        public Task<Either<IFeatureError, ClassificationStatus>> GetClassificationStatusAsync(Guid id);
    }
}
