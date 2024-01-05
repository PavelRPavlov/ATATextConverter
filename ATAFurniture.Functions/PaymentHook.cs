using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CosmosClient = Microsoft.Azure.Cosmos.CosmosClient;
using CosmosClientOptions = Microsoft.Azure.Cosmos.CosmosClientOptions;
using CosmosException = Microsoft.Azure.Cosmos.CosmosException;
using DatabaseResponse = Microsoft.Azure.Cosmos.DatabaseResponse;

namespace ATAFurniture.Functions;

public class PaymentHook
{
    private readonly ILogger _logger;
    private readonly CosmosDbConfiguration _cosmosDbConfiguration;

    public PaymentHook(ILoggerFactory loggerFactory,IOptions<CosmosDbConfiguration> opt)
    {
        _cosmosDbConfiguration = opt.Value;
        _logger = loggerFactory.CreateLogger<PaymentHook>();
    }

    [Function("PaymentHook")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var userIdString = req.Query["id"];
        if (string.IsNullOrEmpty(userIdString) ||
            !Guid.TryParse(userIdString, out var userId))
        {
            _logger.LogError("No user id provided");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        var purchasedCreditsCountString = req.Query["credits"];
        if (string.IsNullOrEmpty(purchasedCreditsCountString) ||
            !int.TryParse(purchasedCreditsCountString, out var purchasedCreditsCount))
        {
            _logger.LogError("No credits count provided");;
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        var containerResult = await EnsureDatabaseContainer(_cosmosDbConfiguration);
        
        if (containerResult.IsFailure)
        {;
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var userResult = await ReadUser(containerResult.Value, userId);
        if (userResult.IsFailure)
        {;
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        _logger.LogInformation("Working with user: {@User}", userResult.Value);

        var updateResult = await UpdateUser(containerResult.Value, userResult.Value, purchasedCreditsCount);

        return req.CreateResponse(updateResult.IsFailure ? HttpStatusCode.BadRequest : HttpStatusCode.OK);
    }
    private async Task<Result<User>> UpdateUser(Container container, User user, int purchasedCreditsCount)
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
            return Result.Failure<User>("Failed to update User");
        }
        
        _logger.LogInformation("Updated user: {@User}", userResponse.Resource);
        return Result.Success<User>(userResponse.Resource);
    }

    private async Task<Result<Container>> EnsureDatabaseContainer(CosmosDbConfiguration cosmosDbConfiguration)
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
            return Result.Failure<Container>("Failed to create CosmosClient");
        }

        DatabaseResponse database;
        try
        {
            database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbConfiguration.DatabaseId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create Database");
            return Result.Failure<Container>("Failed to create Database");
        }

        Container container;
        try
        {
    
            container = await database.Database.CreateContainerIfNotExistsAsync(cosmosDbConfiguration.UserContainerId, "/partitionKey", 400);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create Container");
            return Result.Failure<Container>("Failed to create Container");
        }

        return Result.Success(container);
    }
    
    private async Task<Result<User>> ReadUser(Container container, Guid userId)
    {
        ItemResponse<User> userResponse;
        try
        {
            using var rrrr = container
                .GetItemLinqQueryable<User>()
                .Where(u => u.Id == userId.ToString())
                .ToFeedIterator();
            var usersResult = await rrrr.ReadNextAsync();
            var user = usersResult.FirstOrDefault();
            return Result.Success(user);
            //userResponse = await container.ReadItemAsync<User>(userId.ToString(), new PartitionKey(User.PARTITION_KEY));
            ;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError(ex, "User: {UserId} not found", userId);
            return Result.Failure<User>("User not found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to read User: {UserId}", userId);
            return Result.Failure<User>("Failed to read User");
        }
        
        return Result.Success(userResponse.Resource);
    }
}