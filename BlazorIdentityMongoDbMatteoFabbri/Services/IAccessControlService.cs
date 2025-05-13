using BlazorIdentityMongoDbMatteoFabbri.Data;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public interface IAccessControlService
    {
        List<AccessControlPage> GetAccessControlPagesCollection();

        AccessControlPage GetAccessControlPage(string recordPage);

        void SaveOrUpdateAccessControlPage(AccessControlPage record);

        string DeleteAccessControlPage(string recordPage);
    }

}
