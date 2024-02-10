using PersonalFinanceTracker.Data.Models;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface ILinkService
{
    Link GenerateLink(string endpointName, object? parameters, string rel, string methodName);
}