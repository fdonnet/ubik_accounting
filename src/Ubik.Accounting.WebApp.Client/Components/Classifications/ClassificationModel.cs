using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Enums;
using Ubik.Accounting.Contracts.Classifications.Commands;
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

        public static AddClassificationCommand ToAddClassificationCommand(this ClassificationModel classificationModel)
        {
            return new AddClassificationCommand()
            {
                Code = classificationModel.Code,
                Label = classificationModel.Label,
                Description = classificationModel.Description,
            };
        }

        public static ClassificationModel ToAccountGroupModel(this AddClassificationResult current)
        {
            return new ClassificationModel()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Id = current.Id,
                Version = current.Version
            };
        }

        public static UpdateClassificationCommand ToUpdateClassificationCommand(this ClassificationModel classificationModel)
        {
            return new UpdateClassificationCommand()
            {
                Id = classificationModel.Id,
                Code = classificationModel.Code,
                Label = classificationModel.Label,
                Description = classificationModel.Description,
                Version = classificationModel.Version
            };
        }
    }
}
