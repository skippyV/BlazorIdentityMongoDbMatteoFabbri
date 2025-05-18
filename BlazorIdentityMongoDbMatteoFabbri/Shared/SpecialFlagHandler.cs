using BlazorIdentityMongoDbMatteoFabbri.Data;
using BlazorIdentityMongoDbMatteoFabbri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace BlazorIdentityMongoDbMatteoFabbri.Shared
{
    public class SpecialFlagHandler : AuthorizationHandler<SpecialFlagRequirement>
    {
        List<AccessControlPage> Pages = new List<AccessControlPage>();
        private readonly IAccessControlService _iAccessControlService;
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private UserManager<ApplicationUser> _UserManager;

        public SpecialFlagHandler(IAccessControlService _iService, IHttpContextAccessor _httpContextAccessor, UserManager<ApplicationUser> _userManager)
        {
            _iAccessControlService = _iService;
            _iHttpContextAccessor = _httpContextAccessor;
            _UserManager = _userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecialFlagRequirement requirement)
        {
            Pages = _iAccessControlService.GetAccessControlPagesCollectionAsList(AccessControlPageConstants.Pages);
            string? userIdAsString = null;

            Endpoint? currentEndpoint = null;
            string currentEndpointString = "";
            string spaceValue = " ";

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
            int firstSpaceIndex = currentEndpointString.IndexOf(spaceValue);
            currentEndpointString = currentEndpointString.Substring(1, firstSpaceIndex - 1);

            Task<ApplicationUser?> debugCompareUser = _UserManager.FindByNameAsync("scoobydoo");
            ApplicationUser? compareUser = debugCompareUser.Result;

            System.Security.Claims.ClaimsPrincipal? claimsPrincipal = context?.User;
            if (claimsPrincipal != null)
            {
                if (claimsPrincipal.Identities.Count() > 0)
                {
                    System.Security.Claims.ClaimsIdentity? claimsIdentity = claimsPrincipal.Identities.FirstOrDefault();
                    if (claimsIdentity != null)
                    {
                        IEnumerable<System.Security.Claims.Claim> claims = claimsIdentity.Claims;
                        foreach (var claim in claims)
                        {
                            if (claim.Type.Contains("nameidentifier"))
                            {
                                userIdAsString = claim.Value;
                                break;
                            }
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
