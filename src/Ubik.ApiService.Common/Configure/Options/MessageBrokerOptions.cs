namespace Ubik.ApiService.Common.Configure.Options
{
    public class MessageBrokerOptions : IOptionsPosition
    {
        public const string Position = "MessageBroker";
        public string Host { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; }  = string.Empty;
    }
}


