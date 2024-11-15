using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Webapp.Shared.Features.Classifications.Models
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
        public bool IsExpand { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public AccountGroupModel Clone()
        {
            return new()
            {
                AccountGroupClassificationId = AccountGroupClassificationId,
                Code = Code,
                Description = Description,
                Id = Id,
                IsExpand =IsExpand,
                Label = Label,
                ParentAccountGroupId = ParentAccountGroupId,
                Version = Version
            };
        }
    }

    public static class AccountGroupModelMappers
    {
        public static IEnumerable<AccountGroupModel> ToAccountGroupModels(this IEnumerable<AccountGroupStandardResult> current)
        {
            return current.Select(x => new AccountGroupModel()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                AccountGroupClassificationId = x.AccountGroupClassificationId,
                Version = x.Version,
                IsExpand = true
            });
        }

        public static AccountGroupModel ToAccountGroupModel(this AccountGroupStandardResult current)
        {
            return new()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                ParentAccountGroupId = current.ParentAccountGroupId,
                AccountGroupClassificationId = current.AccountGroupClassificationId,
                Version = current.Version,
                IsExpand = true
            };
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
