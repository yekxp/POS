using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pos_backend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Location
    {
        KE,
        BA
    }
}
