using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.Accounting.WebApp.Client.Components.Accounts;

namespace Ubik.Accounting.WebApp.Client.Components.Classifications
{
    public class ClassificationModel
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;
        [MaxLength(700)]
        public string? Description { get; set; }
        [Required]
        public Guid Version { get; init; }
    }

    public static class ClassificationModelMappers
    {
        public static IEnumerable<ClassificationModel> ToClassificationModel(this IEnumerable<GetAllClassificationsResult> current)
        {
            return current.Select(x => new ClassificationModel()
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
