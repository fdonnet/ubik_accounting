namespace Ubik.Accounting.Api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public required string Label { get; set; }
        public  string? Description { get; set; }
    }
}
