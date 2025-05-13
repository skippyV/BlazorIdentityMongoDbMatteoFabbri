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
            bool continueProcessing = true;

            var castResource = (HttpContext)context.Resource!;
            if (castResource == null)
            {
                continueProcessing = false;
            }
            Endpoint? currentEndpoint = null;
            if (continueProcessing)
            {
                currentEndpoint = castResource.GetEndpoint();
                if (currentEndpoint == null)
                {
                    continueProcessing = false;
                }
            }
            string currentEndpointString = "";
            if (continueProcessing)
            {
                currentEndpointString = currentEndpoint!.DisplayName!;
                int firstSpaceIndex = currentEndpointString.IndexOf(" ");
                currentEndpointString = currentEndpointString.Substring(1, firstSpaceIndex - 1);
            }


            var xx = context?.User;
            if (xx != null)
            {
                if (xx.Identities.Count() > 0)
                {
                    var yy = xx.Identities.FirstOrDefault();
                    if (yy != null)
                    {
                        var zz = yy.Claims;
                        if (zz.Count() > 0)
                        {
                            userIdAsString = zz.FirstOrDefault()!.Value;
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
                                return Task.CompletedTask;
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
