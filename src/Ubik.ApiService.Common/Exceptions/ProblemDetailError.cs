namespace Ubik.ApiService.Common.Exceptions
{
    public class ProblemDetailError
    {
        public required string Code { get; set; }
        public required string FriendlyMsg { get; set; }
        public required string ValueInError { get; set; }
    }
}
