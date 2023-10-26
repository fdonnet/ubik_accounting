using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
