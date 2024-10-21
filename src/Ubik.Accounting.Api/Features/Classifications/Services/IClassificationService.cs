using LanguageExt;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationService
    {
        public Task<Either<IServiceAndFeatureError, ClassificationStatus>> GetClassificationStatusAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, Classification>> AddAsync(Classification classification);
        public Task<Either<IServiceAndFeatureError, Classification>> UpdateAsync(Classification classification);
        public Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
