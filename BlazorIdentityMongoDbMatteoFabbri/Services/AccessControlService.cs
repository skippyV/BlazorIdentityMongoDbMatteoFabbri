using BlazorIdentityMongoDbMatteoFabbri.Data;
using MongoDB.Driver;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public class AccessControlService : IAccessControlService
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase iMongoDatabase = null;
        private IMongoCollection<AccessControlPage> iCollectionAccessControlPages = null;

        public AccessControlService()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            iMongoDatabase = mongoClient.GetDatabase("AccessControl");
            iCollectionAccessControlPages = iMongoDatabase.GetCollection<AccessControlPage>("Pages");
        }

        public List<AccessControlPage> GetAccessControlPagesCollection()
        {
            return iCollectionAccessControlPages.Find(FilterDefinition<AccessControlPage>.Empty).ToList();
        }

        public AccessControlPage GetAccessControlPage(string pageRecordName)
        {
            AccessControlPage? page = iCollectionAccessControlPages.AsQueryable().Where(r => r.pageName == pageRecordName).FirstOrDefault();
            return page;
        }

        public void SaveOrUpdateAccessControlPage(AccessControlPage pageRecordName)
        {
            throw new NotImplementedException();
        }
        public string DeleteAccessControlPage(string pageRecordName)
        {
            throw new NotImplementedException();
        }
    }
}
