using BlazorIdentityMongoDbMatteoFabbri.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public class AccessControlService : IAccessControlService
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase iMongoDatabase = null;

        public AccessControlService()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            iMongoDatabase = mongoClient.GetDatabase("AccessControl");
        }

        public void CreateCollection(string collectionName)
        {
            iMongoDatabase.CreateCollection(collectionName); // if collection exists then it is not created
        }

        public List<AccessControlPage> GetAccessControlPagesCollectionAsList(string collectionName)
        {
            IMongoCollection<AccessControlPage> iCollectionAccessControlPages = iMongoDatabase.GetCollection<AccessControlPage>(collectionName);
            return iCollectionAccessControlPages.Find(FilterDefinition<AccessControlPage>.Empty).ToList();
        }

        public AccessControlPage GetAccessControlPage(string pageRecordName, string collectionName)
        {
            IMongoCollection<AccessControlPage> pages = iMongoDatabase.GetCollection<AccessControlPage>(collectionName);

            FilterDefinition<AccessControlPage> filter = Builders<AccessControlPage>.Filter.Eq(AccessControlPageConstants.pageName, pageRecordName);

            //AccessControlPage page = pages.Find(filter).FirstOrDefault();
            AccessControlPage page = pages.Find<AccessControlPage>(filter).FirstOrDefault();

            return page;
        }

        public void AddAccessControlPage(AccessControlPage page,  string collectionName)
        {
            try
            {
                IMongoCollection<AccessControlPage> pages = iMongoDatabase.GetCollection<AccessControlPage>(collectionName);

                FilterDefinition<AccessControlPage> filter = Builders<AccessControlPage>.Filter.Eq(nameof(AccessControlPage.pageName), page.pageName);

                Task<IAsyncCursor<AccessControlPage>> doesPageExist = pages.FindAsync(c => c.pageName.Equals(page.pageName));
                doesPageExist.Wait();

                if (doesPageExist.Result.FirstOrDefault() == null) // if page does not already exist
                {
                    pages.InsertOne(page);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void UpdateAccessControlPage(AccessControlPage pageRecordName)
        {
            throw new NotImplementedException();
        }

        public string DeleteAccessControlPage(string pageRecordName)
        {
            throw new NotImplementedException();
        }

    }
}
