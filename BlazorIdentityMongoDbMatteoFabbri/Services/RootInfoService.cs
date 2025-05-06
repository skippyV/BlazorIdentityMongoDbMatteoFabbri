namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public class RootInfoService : IRootInfoService
    {
        private readonly string rootUserName;
        public string GetRootUserName()
        {
            return rootUserName;
        }
        public RootInfoService(string name)
        {
            rootUserName = name;
        }
    }
}
