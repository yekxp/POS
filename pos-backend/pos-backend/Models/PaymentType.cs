using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pos_backend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentType
    {
        Cash = 0,
        CreditCard = 1
    }
}
