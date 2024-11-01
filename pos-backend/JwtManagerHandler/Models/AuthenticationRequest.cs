using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtManagerHandler.Models
{
    public class AuthenticationRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

        [JsonIgnore]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
    }
}
