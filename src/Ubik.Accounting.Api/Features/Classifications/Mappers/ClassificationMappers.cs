using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Mappers
{
    public static class ClassificationMappers
    {
        public static IEnumerable<GetAllClassificationsResult> ToGetAllClassificationsResult(this IEnumerable<Classification> current)
        {
            return current.Select(x => new GetAllClassificationsResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }
    }
}
