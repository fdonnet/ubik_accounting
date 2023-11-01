using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<ResultT<Classification>> GetAsync(Guid id);
        Task<bool> IfExistsAsync(Guid id);
    }
}
