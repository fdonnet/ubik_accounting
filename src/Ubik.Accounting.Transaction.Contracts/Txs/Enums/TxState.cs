using System.Text.Json.Serialization;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TxState
    {
        Submitted,
        WaitingForTaxValidation,
        TaxValidated,
        Confirmed,
        Rejected
    }
}
