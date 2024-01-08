using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ATAFurniture.Server.Components;

/// <summary>
/// Base class for UserClaims component.
/// Retrieves claims present in the ID Token issued by Azure AD.
/// </summary>
public class UserClaimsBase : ComponentBase
{

    [Inject]
    private ILogger<UserClaimsBase> _logger { get; set; }
    
    [Inject]
    IConfiguration Configuration { get; set; }

    protected string secretValue;

    protected override async Task OnInitializedAsync()
    {
        secretValue = await ReadSecret();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        _logger.LogInformation("OnAfterRender for UserClaims component");
    }

    // KeyVault does not support RBAC
    private async Task<string> ReadSecret()
    {
        // SecretClientOptions options = new SecretClientOptions()
        // {
        //     Retry =
        //     {
        //         Delay= TimeSpan.FromSeconds(2),
        //         MaxDelay = TimeSpan.FromSeconds(16),
        //         MaxRetries = 5,
        //         Mode = RetryMode.Exponential
        //     }
        // };
        // SecretClient client;
        // try
        // {
        //     client = new SecretClient(
        //         new Uri("https://atafurniture-keys.vault.azure.net/"),
        //         // not working for KeyVault -> new ManagedIdentityCredential(Configuration.GetSection("ManagedIdentityClientId").Value),
        //         new DefaultAzureCredential(
        //             new DefaultAzureCredentialOptions()
        //             {
        //                 ManagedIdentityClientId = "5f996b60-56be-415b-b6b3-442400bc6c3f"
        //             }),
        //         options);
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogCritical(ex, "could not initialize a SecretClient");
        //     throw;
        // }
        //
        // var keySecret = await client.GetSecretAsync("BlazorAppTenantId");
        //
        // return keySecret.Value.Value;
        return "KeyVault not working with RBAC authentication";
    }
}