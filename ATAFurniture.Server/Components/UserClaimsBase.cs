using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ATAFurniture.Server.Components;

/// <summary>
/// Base class for UserClaims component.
/// Retrieves claims present in the ID Token issued by Azure AD.
/// </summary>
public class UserClaimsBase : ComponentBase
{
    // AuthenticationStateProvider service provides the current user's ClaimsPrincipal data.
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    private ILogger<UserClaimsBase> _logger { get; set; }
    
    [Inject]
    IConfiguration Configuration { get; set; }

    protected string secretValue;

    protected string _authMessage;
    protected IEnumerable<Claim> _claims = Enumerable.Empty<Claim>();

    // Defines list of claim types that will be displayed after successfull sign-in.
    private string[] printClaims = { "name", "idp", "oid", "jobTitle", "emails" };

    protected override async Task OnInitializedAsync()
    {
        await GetClaimsPrincipalData();
        secretValue = await ReadSecret();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        _logger.LogInformation("OnAfterRender for UserClaims component");
    }

    /// <summary>
    /// Retrieves user claims for the signed-in user.
    /// </summary>
    /// <returns></returns>
    private async Task GetClaimsPrincipalData()
    {
        // Gets an AuthenticationState that describes the current user.
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        // Checks if the user has been authenticated.
        if (user.Identity.IsAuthenticated)
        {
            _authMessage = $"{user.Identity.Name} is authenticated.";

            // Sets the claims value in _claims variable.
            // The claims mentioned in printClaims variable are selected only.
            _claims = user.Claims.Where(x => printClaims.Contains(x.Type));
        }
        else
        {
            _authMessage = "The user is NOT authenticated.";
        }
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