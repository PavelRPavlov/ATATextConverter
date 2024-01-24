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
    public User User { get; private set; }

    public UserContextService(
        AuthenticationStateProvider authenticationStateProvider,
        CosmosDbContext cosmosDbContext,
        ILogger<UserContextService> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _cosmosDbContext = cosmosDbContext;
        _logger = logger;
        ExtractUserIdentity().ConfigureAwait(false);
    }
    
    public async Task AddCredits(int count, bool countResets = false)
    {
        if (countResets)
        {
            User.CreditResets++;
        }
        _logger.LogInformation("Adding {CreditCount} credits to user {Id}", count, User.Id);
        await _cosmosDbContext.AddCredits(User.Id, count, countResets);
        User.CreditsCount += count;
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
            
            var dbUser = await _cosmosDbContext.GetUser(userId) ?? await _cosmosDbContext.CreateUser(userId, 10);
            
            dbUser = await SyncUserClaimsAsync(dbUser, userName, userEmail, mobileNumber, companyName);
            
            User = dbUser; 
        }
    }

    private async Task<User> SyncUserClaimsAsync(User dbUser, string? userName, string? userEmail, string? mobileNumber, string? companyName)
    {
        var shouldUpdate = false;
        if (!string.IsNullOrEmpty(userName) && dbUser.Name != userName)
        {
            _logger.LogInformation("Updating user info for {Id} ==> Property NAME to {UserName}", dbUser.Id, userName);
            dbUser.Name = userName;
            shouldUpdate = true;
        }
        if (!string.IsNullOrEmpty(userEmail) && dbUser.Email != userEmail)
        {
            _logger.LogInformation("Updating user info for {Id} ==> Property EMAIL to {UserEmail}", dbUser.Id, userEmail);
            dbUser.Email = userEmail;
            shouldUpdate = true;
        }
        if (!string.IsNullOrEmpty(mobileNumber) && dbUser.MobileNumber != mobileNumber)
        {
            _logger.LogInformation("Updating user info for {Id} ==> Property MOBILE_NUMBER to {UserMobileNumber}", dbUser.Id, mobileNumber);
            dbUser.MobileNumber = mobileNumber;
            shouldUpdate = true;
        }
        if (!string.IsNullOrEmpty(companyName) && dbUser.CompanyName != companyName)
        {
            _logger.LogInformation("Updating user info for {Id} ==> Property COMPANY_NAME to {UserCompanyName}", dbUser.Id, companyName);
            dbUser.CompanyName = companyName;
            shouldUpdate = true;
        }
        
        if(shouldUpdate)
        {
            await _cosmosDbContext.UpdateUser(dbUser);
        }

        return dbUser;
    }

    public async Task ConsumeSingleCredit()
    {
        await _cosmosDbContext.RemoveCredits(User.Id, 1);
        User.CreditsCount--;
    }

    public async Task UpdateSelectedCompanyAsync(SupportedCompany targetCompany)
    {
        User.LastSelectedCompany = targetCompany;
        await _cosmosDbContext.UpdateSelectedCompany(User.Id, targetCompany);
    }

    public async ValueTask<SupportedCompany> GetPreviouslySelectedTargetCompanyAsync()
    {
        var user = await _cosmosDbContext.GetUser(User.Id);
        return user?.LastSelectedCompany;
    }
}