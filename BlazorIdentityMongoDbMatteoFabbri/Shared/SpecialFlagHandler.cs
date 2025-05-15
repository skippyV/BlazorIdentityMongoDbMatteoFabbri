using BlazorIdentityMongoDbMatteoFabbri.Data;
using BlazorIdentityMongoDbMatteoFabbri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace BlazorIdentityMongoDbMatteoFabbri.Shared
{
    public class SpecialFlagHandler : AuthorizationHandler<SpecialFlagRequirement>
    {
        List<AccessControlPage> Pages = new List<AccessControlPage>();
        private readonly IAccessControlService _iAccessControlService;
        private readonly IHttpContextAccessor _iHttpContextAccessor;

        public SpecialFlagHandler(IAccessControlService _iService, IHttpContextAccessor httpContextAccessor)
        {
            _iAccessControlService = _iService;
            _iHttpContextAccessor = httpContextAccessor;
        }

        // THIS HANDLER NOW IS CALLED 3 TIMES! This can't be the correct approach...
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecialFlagRequirement requirement)
        {
            Pages = _iAccessControlService.GetAccessControlPagesCollection();
            string? userIdAsString = null;

            Endpoint? currentEndpoint = null;
            string currentEndpointString = "";

            if (context.Resource is HttpContext httpContext)
            {
                currentEndpoint = httpContext.GetEndpoint();                
            }
            else if (context.Resource == null && _iHttpContextAccessor != null && _iHttpContextAccessor.HttpContext != null)
            {
                currentEndpoint = _iHttpContextAccessor.HttpContext.GetEndpoint();
            }
            else
            {
                Console.WriteLine("ERROR - WHAT THE HECK IS GOING ON NOW?");
                return Task.CompletedTask;
            }

            currentEndpointString = currentEndpoint!.DisplayName!;
            int firstSpaceIndex = currentEndpointString.IndexOf(" ");
            currentEndpointString = currentEndpointString.Substring(1, firstSpaceIndex - 1);

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
