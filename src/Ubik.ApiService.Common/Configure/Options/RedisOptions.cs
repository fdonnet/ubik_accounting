namespace Ubik.ApiService.Common.Configure.Options
{
    public class RedisOptions : IOptionsPosition
    {
        public const string Position = "RedisCache";
        public string ConnectionString { get; set; } = string.Empty;
    }
}
