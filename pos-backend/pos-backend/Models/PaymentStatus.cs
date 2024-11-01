using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pos_backend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Refunded,
        Cancelled
    }
}
