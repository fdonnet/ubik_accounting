using System.Text.Json.Serialization;

namespace Ubik.ApiService.DB.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountDomain
    {
        Asset,
        Liability,
        Charge,
        Income,
        Neutral
    }
}
