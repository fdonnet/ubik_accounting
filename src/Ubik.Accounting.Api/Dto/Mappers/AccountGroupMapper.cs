using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Dto.Mappers
{
    public static class AccountGroupMapper
    {
        public static AccountGroupDto ToAccountGroupDto(this AccountGroup accountGroup)
        {
            return new AccountGroupDto()
            {
                Id = accountGroup.Id,
                Code = accountGroup.Code,
                Description = accountGroup.Description,
                Label = accountGroup.Label,
                ParentAccountGroupId = accountGroup.ParentAccountGroupId
            };
        }
    }
}
