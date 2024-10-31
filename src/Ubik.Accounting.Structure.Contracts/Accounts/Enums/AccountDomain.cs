using System.Text.Json.Serialization;

namespace Ubik.Accounting.Structure.Contracts.Accounts.Enums
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
