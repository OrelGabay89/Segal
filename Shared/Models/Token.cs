using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shared.Models
{
    public class Token
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("token_type")]
        public string TokenType { get; set; }

        [BsonElement("access_token")]
        public string AccessToken { get; set; }

        [BsonElement("scope")]
        public string Scope { get; set; }

        [BsonElement("expires_in")]
        public int ExpiresIn { get; set; }

        [BsonElement("consented_on")]
        public long ConsentedOn { get; set; }

        [BsonElement("refresh_token")]
        public string RefreshToken { get; set; }

        [BsonElement("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
        
        [BsonElement("status")]
        public string Status { get; set; } = "Active"; // Default to Active
    }
}
