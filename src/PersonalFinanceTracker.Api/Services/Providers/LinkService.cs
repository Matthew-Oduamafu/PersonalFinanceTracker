using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models;

namespace PersonalFinanceTracker.Api.Services.Providers;

internal sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    : ILinkService
{
    public Link GenerateLink(string endpointName, object? parameters, string rel, string methodName)
    {
        return new Link
        (
            linkGenerator.GetUriByName(httpContextAccessor.HttpContext!, endpointName, parameters),
            rel,
            methodName
        );
    }
}