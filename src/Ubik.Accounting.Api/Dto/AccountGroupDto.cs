namespace Ubik.Accounting.Api.Dto
{
    public class AccountGroupDto
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public Guid? ParentAccountGroupId { get; set; }
    }
}
