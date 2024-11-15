using System.Text.Json.Serialization;

namespace Ubik.Accounting.Transaction.Contracts.Entries.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DebitCredit
    {
        Debit,
        Credit
    }
}
