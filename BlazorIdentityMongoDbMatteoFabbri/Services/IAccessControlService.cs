using BlazorIdentityMongoDbMatteoFabbri.Data;
using MongoDB.Bson.Serialization;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public interface IAccessControlService
    {
        void CreateCollection(string collectionName);

        List<AccessControlPage> GetAccessControlPagesCollectionAsList(string collectionName);

        AccessControlPage GetAccessControlPage(string pageRecordName, string collectionName);

        public void AddAccessControlPage(AccessControlPage page, string collectionName);
        
        void UpdateAccessControlPage(AccessControlPage record);

        string DeleteAccessControlPage(string recordPage);
    }

}
