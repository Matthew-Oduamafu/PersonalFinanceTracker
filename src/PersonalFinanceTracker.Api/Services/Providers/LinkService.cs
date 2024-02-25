using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models;

namespace PersonalFinanceTracker.Api.Services.Providers;

internal sealed class LinkService : ILinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public Link GenerateLink(string endpointName, object? parameters, string rel, string methodName)
    {
        return new Link
        (
            _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext!, endpointName, parameters),
            rel,
            methodName
        );
    }
}