using System.Text.Json.Serialization;

namespace Ubik.Accounting.Structure.Contracts.Accounts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountCategory
    {
        General,
        Liquidity,
        DebtorCreditor
    }
}
