using System.Text.Json.Serialization;

namespace Ubik.Accounting.Contracts.Accounts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountCategory
    {
        General,
        Liquidity,
        DebtorCreditor
    }
}
