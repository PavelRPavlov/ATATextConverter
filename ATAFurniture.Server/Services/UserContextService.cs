using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace ATAFurniture.Server.Services;

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

    public UserContextService(
        AuthenticationStateProvider authenticationStateProvider,
        CosmosDbContext cosmosDbContext,
        ILogger<UserContextService> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _cosmosDbContext = cosmosDbContext;
        _logger = logger;
        CreateUserContext().ConfigureAwait(false);
    }

    private async Task CreateUserContext()
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

            var dbUser = await _cosmosDbContext.GetUser(userId);
            var creditCount = dbUser.CreditsCount;

            CreditCount = creditCount;
        }
    }
}