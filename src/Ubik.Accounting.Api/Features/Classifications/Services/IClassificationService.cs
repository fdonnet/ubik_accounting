using LanguageExt;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<Either<IServiceAndFeatureException, Classification>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureException, IEnumerable<Account>>> GetClassificationAccountsAsync(Guid id);
        public Task<Either<IServiceAndFeatureException, IEnumerable<Account>>> GetClassificationAccountsMissingAsync(Guid id);
        public Task<Either<IServiceAndFeatureException, ClassificationStatus>> GetClassificationStatusAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<Either<IServiceAndFeatureException, Classification>> AddAsync(Classification classification);
        public Task<Either<IServiceAndFeatureException, Classification>> UpdateAsync(Classification classification);
        public Task<Either<IServiceAndFeatureException, IEnumerable<AccountGroup>>> DeleteAsync(Guid id);
    }
}
