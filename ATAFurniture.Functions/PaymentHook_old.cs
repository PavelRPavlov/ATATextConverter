using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


[assembly: FunctionsStartup(typeof(ATAFurniture.Functions.Startup))]

namespace ATAFurniture.Functions;

public partial class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.Configure<CosmosDbConfiguration>(builder.GetContext().Configuration.GetSection("CosmosDb"));
    }
}

public static class PaymentHook_old
{
    private static ILogger _logger;

    [FunctionName("PaymentHook")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, IOptions<CosmosDbConfiguration> cosmosDbConfigValue)   
    {
        _logger = log;
        var userId = req.Query["id"];
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("No user id provided");
            return new BadRequestResult();
        }
        
        var purchasedCreditsCountString = req.Query["credits"];
        var purchasedCreditsCount = 0;
        if (string.IsNullOrEmpty(purchasedCreditsCountString) &&
            !int.TryParse(purchasedCreditsCountString, out purchasedCreditsCount))
        {
            _logger.LogError("No credits count provided");
            return new BadRequestResult();
        }
        
        var containerResult = await EnsureDatabaseContainer(cosmosDbConfigValue.Value);
        
        if (!containerResult.Success)
        {
            return new BadRequestResult();
        }
        
        var userResult = await ReadUser(containerResult.Container, userId);
        if (!userResult.Success)
        {
            return new BadRequestResult();
        }
        
        _logger.LogInformation("Working with user: {@User}", userResult.User);
        
        var updateResult = await UpdateUser(containerResult.Container, userResult.User, purchasedCreditsCount);
        
        if (!updateResult.Success)
        {
            return new BadRequestResult();
        }
        
        _logger.LogInformation("Updated user: {@User}", updateResult.user);
        return new OkResult();
    }

    private static async Task<(bool Success, User user)> UpdateUser(Container container, User user, int purchasedCreditsCount)
    {
        user.AddCredits(purchasedCreditsCount);
        ItemResponse<User> userResponse;
        try
        {
            userResponse = await container.ReplaceItemAsync(user, user.Id, new PartitionKey(User.PARTITION_KEY));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update User: {UserId}", user.Id);
            return (false, null);
        }
        
        _logger.LogInformation("Updated user: {@User}", userResponse.Resource);
        return (true, userResponse.Resource);
    }

    private static async Task<(bool Success, Container Container)> EnsureDatabaseContainer(CosmosDbConfiguration cosmosDbConfiguration)
    {
        CosmosClient client;
        try
        {
            client = new CosmosClient(
                cosmosDbConfiguration.EndpointUri,
                cosmosDbConfiguration.PrimaryKey,
                new CosmosClientOptions
                { 
                    ApplicationName = "ATAFurniture Converter" 
                });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create CosmosClient");
            return (false, null);
        }

        DatabaseResponse database;
        try
        {
            database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbConfiguration.DatabaseId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create Database");
            return (false, null);
        }

        Container container;
        try
        {
    
            container = await database.Database.CreateContainerIfNotExistsAsync(cosmosDbConfiguration.UserContainerId, "/partitionKey", 400);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create Container");
            return (false, null);
        }

        return (true, container);
    }
    
    private static async Task<(bool Success, User User)> ReadUser(Container container, string userId)
    {
        ItemResponse<User> userResponse;
        try
        {
            userResponse = await container.ReadItemAsync<User>(userId, new PartitionKey(User.PARTITION_KEY));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError(ex, "User: {UserId} not found", userId);
            return (false, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to read User: {UserId}", userId);
            return (false, null);
        }
        
        return (true, userResponse?.Resource);
    }
}