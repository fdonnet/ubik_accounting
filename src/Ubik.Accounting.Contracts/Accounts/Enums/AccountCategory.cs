using System.Text.Json.Serialization;

namespace Ubik.ApiService.DB.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountCategory
    {
        General,
        Liquidity,
        DebtorCreditor
    }
}
