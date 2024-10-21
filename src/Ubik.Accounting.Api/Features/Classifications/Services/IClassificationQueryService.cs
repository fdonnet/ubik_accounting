using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationQueryService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id);
    }
}
