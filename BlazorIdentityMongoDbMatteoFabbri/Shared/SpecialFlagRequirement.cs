using Microsoft.AspNetCore.Authorization;

namespace BlazorIdentityMongoDbMatteoFabbri.Shared
{
    public class SpecialFlagRequirement : IAuthorizationRequirement
    {
        public bool SpecialFlag { get; set; }
    }
}
