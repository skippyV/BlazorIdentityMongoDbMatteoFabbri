using BlazorIdentityMongoDbMatteoFabbri.Data;
using BlazorIdentityMongoDbMatteoFabbri.Services;
using Microsoft.AspNetCore.Authorization;


namespace BlazorIdentityMongoDbMatteoFabbri.Shared
{
    public class SpecialFlagHandler : AuthorizationHandler<SpecialFlagRequirement>
    {
        List<AccessControlPage> Pages = new List<AccessControlPage>();
        private readonly IAccessControlService _iAccessControlService;
        public SpecialFlagHandler(IAccessControlService _iService)
        {
            _iAccessControlService = _iService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecialFlagRequirement requirement)
        {
            Pages = _iAccessControlService.GetAccessControlPagesCollection();
            string? userIdAsString = null;
            //bool continueProcessing = true;

            Endpoint? currentEndpoint = null;
            string currentEndpointString = "";

            if (context.Resource is HttpContext httpContext)
            {
                currentEndpoint = httpContext.GetEndpoint();
                currentEndpointString = currentEndpoint!.DisplayName!;
                int firstSpaceIndex = currentEndpointString.IndexOf(" ");
                currentEndpointString = currentEndpointString.Substring(1, firstSpaceIndex - 1);
            }

            if (context.Resource == null) // THIS HANDLER IS BEING CALLED TWICE !! SMH
            {
                return Task.CompletedTask;
            }

            System.Security.Claims.ClaimsPrincipal? claimsPrincipal = context?.User;
            if (claimsPrincipal != null)
            {
                if (claimsPrincipal.Identities.Count() > 0)
                {
                    System.Security.Claims.ClaimsIdentity? claimsIdentity = claimsPrincipal.Identities.FirstOrDefault();
                    if (claimsIdentity != null)
                    {
                        IEnumerable<System.Security.Claims.Claim> claims = claimsIdentity.Claims;
                        if (claims.Count() > 0)
                        {
                            userIdAsString = claims.FirstOrDefault()!.Value;
                        }
                    }
                }

                if (userIdAsString != null)
                {
                    foreach (var page in Pages)
                    {
                        if (page.pageName == currentEndpointString)
                        {
                            if (page.allowedUsers.Contains(userIdAsString))
                            {
                                // requirement.SpecialFlag = true;  // See comments in SpecialFlagRequirement.cs
                                context!.Succeed(requirement);
                                //return Task.CompletedTask;
                            }
                            break;
                        }                        
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
