using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlazorIdentityMongoDbMatteoFabbri.Data
{
    public class AccessControlPage
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string pageName { get; set; } = "";
        public string[] allowedUsers { get; set; } = [];
    }
}
