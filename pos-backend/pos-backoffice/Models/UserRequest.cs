using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace pos_backoffice.Models
{
    public class UserRequest
    {
        public required string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [EnumDataType(typeof(Role))]
        public required string Role { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }

        [Compare("Password")]
        public required string ConfirmPassword { get; set; }
    }
}
