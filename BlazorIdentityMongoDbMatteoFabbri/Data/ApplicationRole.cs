using AspNetCore.Identity.Mongo.Model;

namespace BlazorIdentityMongoDbMatteoFabbri.Data
{
    public class ApplicationRole : MongoRole
    {
        public ApplicationRole() { }
        public ApplicationRole(string name)
            : base(name)
        {
        }
    }
}
