using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.DataAccess;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace Kroiko.Domain;

public class UserContextService
{
    private const string UserIdClaimName = "oid";
    private const string UserNameClaimName = "name";
    private const string EmailClaimName = "emails";
    private const string MobileNumberClaimName = "extension_MobileNumber";
    private const string CompanyNameClaimName = "extension_CompanyName";

    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly CosmosDbContext _cosmosDbContext;
    private readonly ILogger<UserContextService> _logger;
    
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string MobileNumber { get; private set; }
    public string CompanyName { get; private set; }
    public int CreditCount { get; private set; }
    public int CreditResets { get; private set; }
    public SupportedCompany LastSelectedCompany { get; private set; }

    public UserContextService(
        AuthenticationStateProvider authenticationStateProvider,
        CosmosDbContext cosmosDbContext,
        ILogger<UserContextService> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _cosmosDbContext = cosmosDbContext;
        _logger = logger;
        ExtractUserIdentity().ConfigureAwait(false);
        EnrichUserContextWithDbData().ConfigureAwait(false);
    }
    
    public async Task AddCredits(int count, bool countResets = false)
    {
        if (countResets)
        {
            CreditResets++;
        }
        _logger.LogInformation("Adding {CreditCount} credits to user {Id}", count, Id);
        await _cosmosDbContext.AddCredits(Id, count, countResets);
        CreditCount += count;
    }

    public async Task EnrichUserContextWithDbData()
    {
        var dbUser = await _cosmosDbContext.GetUser(Id) ?? await _cosmosDbContext.CreateUser(Id, 10);
        CreditCount = dbUser.CreditsCount;
        LastSelectedCompany = dbUser.LastSelectedCompany;
    }

    private async Task ExtractUserIdentity()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type.Equals(UserIdClaimName))?.Value;
            var userName = user.Claims.FirstOrDefault(c => c.Type.Equals(UserNameClaimName))?.Value;
            var userEmail = user.Claims.FirstOrDefault(c => c.Type.Equals(EmailClaimName))?.Value;
            var mobileNumber = user.Claims.FirstOrDefault(c => c.Type.Equals(MobileNumberClaimName))?.Value;
            var companyName = user.Claims.FirstOrDefault(c => c.Type.Equals(CompanyNameClaimName))?.Value;

            Id = userId;
            Name = userName;
            Email = userEmail;
            MobileNumber = mobileNumber;
            CompanyName = companyName;
        }
    }

    public async Task ConsumeSingleCredit()
    {
        await _cosmosDbContext.RemoveCredits(Id, 1);
        CreditCount--;
    }

    public async Task UpdateSelectedCompanyAsync(SupportedCompany targetCompany)
    {
        LastSelectedCompany = targetCompany;
        await _cosmosDbContext.UpdateSelectedCompany(Id, targetCompany);
    }

    public async ValueTask<SupportedCompany> GetPreviouslySelectedTargetCompanyAsync()
    {
        var user = await _cosmosDbContext.GetUser(Id);
        return user?.LastSelectedCompany;
    }
}