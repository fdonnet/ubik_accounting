using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationQueryService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
    }
}
