using System;
using System.Threading.Tasks;
using ATAFurniture.Server.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using User = ATAFurniture.Server.Models.User;

namespace ATAFurniture.Server.Services;

public class CosmosDbContext(
    ILogger<CosmosDbContext> logger,
    IOptions<CosmosDbConfiguration> cosmosDbConfig)
{
    private CosmosClient _cosmosClient = null;
    private Database _database = null;
    private Container _container = null;

    public async Task<User> GetUser(string userId)
    {
        await EnsureDatabaseContainer();
        ItemResponse<User> userResponse;
        try
        {
            userResponse = await _container.ReadItemAsync<User>(userId, new PartitionKey(User.PARTITION_KEY));
        }
        catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogError("User with id {UserId} not found", userId);
            return null;
        }
        return userResponse.Resource;
    }

    private async Task EnsureDatabaseContainer()
    {
        var cosmosConfig = cosmosDbConfig?.Value;
        if (cosmosConfig == null)
        {
            logger.LogError("CosmosDbConfiguration is null");
            throw new ArgumentNullException(nameof(cosmosDbConfig));
        }
        _cosmosClient ??= new CosmosClient(
            cosmosConfig.EndpointUri,
            cosmosConfig.PrimaryKey,
            new CosmosClientOptions
            { 
                ApplicationName = "ATAFurniture Converter" 
            });
        _database ??= await _cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseId);

        _container ??= await _database.CreateContainerIfNotExistsAsync(cosmosConfig.UserContainerId, "/partitionKey", 400);
    }
}