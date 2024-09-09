using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Commands;

namespace Ubik.Accounting.WebApp.Client.Components.Classifications
{
    public class AccountGroupModel
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
        public Guid? ParentAccountGroupId { get; set; }
        [Required]
        public Guid AccountGroupClassificationId { get; set; }
        [Required]
        public Guid Version { get; set; }
        public bool IsExpand { get; set; } = true;
    }

    public static class AccountGroupModelMappers
    {
        public static IEnumerable<AccountGroupModel> ToAccountGroupModel(this IEnumerable<GetAllAccountGroupsResult> current)
        {
            return current.Select(x => new AccountGroupModel()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                AccountGroupClassificationId = x.AccountGroupClassificationId,
                Version = x.Version
            });
        }

        public static AddAccountGroupCommand ToAddAccountGroupCommand(this AccountGroupModel accountGroupModel)
        {
            return new AddAccountGroupCommand()
            {
                Code = accountGroupModel.Code,
                Label = accountGroupModel.Label,
                Description = accountGroupModel.Description,
                AccountGroupClassificationId= accountGroupModel.AccountGroupClassificationId,
                ParentAccountGroupId= accountGroupModel.ParentAccountGroupId
            };
        }

        public static UpdateAccountGroupCommand ToUpdateAccountGroupCommand(this AccountGroupModel accountGroupModel)
        {
            return new UpdateAccountGroupCommand()
            {
                Id = accountGroupModel.Id,
                Code = accountGroupModel.Code,
                Label = accountGroupModel.Label,
                Description = accountGroupModel.Description,
                AccountGroupClassificationId = accountGroupModel.AccountGroupClassificationId,
                ParentAccountGroupId = accountGroupModel.ParentAccountGroupId,
                Version = accountGroupModel.Version
            };
        }
    }
}
