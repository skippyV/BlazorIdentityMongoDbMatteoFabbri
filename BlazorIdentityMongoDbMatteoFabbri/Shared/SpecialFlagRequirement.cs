using Microsoft.AspNetCore.Authorization;

namespace BlazorIdentityMongoDbMatteoFabbri.Shared
{
    // I'm not sure where I was going with this Requirement definition.
    // Originally I was thinking that one could provide a parameter to the component attribute
    // e.g. @attribute [Authorize(Policy = "SpecialFlagPolicy")]
    // would somehow receive an input parameter from the component which could
    // then be used by the Attribute to pass the information into the policy logic.
    //  e.g. @attribute [Authorize(Policy = "SpecialFlagPolicy", myVariableWhosValueIsSetFromPageCode)]
    // By receiving the value of the property and using that value within the auth policy.
    //
    // But now I don't think that is possible...
    // So commenting out this bool for now and leaving this note.
    // Until my knowldege of this improves. :)
    //
    // The closest I've seen anything similar to what I described is the Stackoverflow answer from Michał Turczyn
    // to question: https://stackoverflow.com/questions/78958957/asp-net-core-8-custom-parameterized-authorization-atttribute


    public class SpecialFlagRequirement : IAuthorizationRequirement
    {
        //public bool SpecialFlag { get; set; } 
    }
}
