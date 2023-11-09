using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<Either<IServiceAndFeatureException, Classification>> GetAsync(Guid id);
        
        //Task<bool> IfExistsAsync(Guid id);
        //public Task<bool> IfExistsAsync(string code);
        //public Task<ResultT<Classification>> AddAsync(Classification classification);
        //public Task<ResultT<Classification>> UpdateAsync(Classification classification);
        //public Task<ResultT<bool>> ExecuteDeleteAsync(Guid id);
    }
}
