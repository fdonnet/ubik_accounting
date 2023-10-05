using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Api.Dto
{
    public class AccountDtoForAdd
    {
        [StringLength(20)]
        public required string Code { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public string? Description { get; set; }
        public Guid AccountGroupId { get; set; }
    }
}
