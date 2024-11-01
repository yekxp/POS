using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace pos_backoffice.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public required string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public required string Email { get; set; }
        public Role Role { get; set; }

        [JsonIgnore]
        public string? PasswordHash { get; set; }
    }
}
