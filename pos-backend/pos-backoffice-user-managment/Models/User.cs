using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace pos_backoffice_user_managment.Models
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

        [BsonRepresentation(BsonType.String)]
        public Role Role { get; set; }

        [JsonIgnore]
        public string? PasswordHash { get; set; }
    }
}
