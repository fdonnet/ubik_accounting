using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.WebApp.Client.Components.Classifications
{
    public class AccountGroupModel
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;
        [MaxLength(700)]
        public string? Description { get; set; }
        public Guid? ParentAccountGroupId { get; set; }
        [Required]
        public Guid AccountGroupClassificationId { get; set; }
        [Required]
        public Guid Version { get; set; }
    }

    public static class AccountGroupModelMappers
    {
        public static IEnumerable<AccountGroupModel> ToAccountGroupModel(this IEnumerable<GetAllAccountGroupsResult> current)
        {
            return current.Select(x => new AccountGroupModel()
            {
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                AccountGroupClassificationId = x.AccountGroupClassificationId,
                Version = x.Version
            });
        }
    }
}
