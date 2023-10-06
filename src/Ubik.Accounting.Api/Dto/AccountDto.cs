using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Dto;

namespace Ubik.Accounting.Api.Dto
{
   public class AccountDto : IDtoWithId
    {
        public Guid Id { get; set; }
        [StringLength(20)]
        public required string Code { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public string? Description { get; set; }
        public Guid AccountGroupId { get; set; }
    }
}
