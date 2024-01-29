﻿using Kroiko.Domain.CellsExtracting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kroiko.Domain.DataAccess;

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
    
    public async Task<User> CreateUser(string userId, int initialCreditCount)
    {
        await EnsureDatabaseContainer();
        var user = new User
        {
            Id = userId,
            PartitionKey = User.PARTITION_KEY,
            CreditsCount = initialCreditCount
        };
        ItemResponse<User> userResponse;
        try
        {
            userResponse = await _container.CreateItemAsync(user, new PartitionKey(User.PARTITION_KEY));
            logger.LogInformation("Created user {Id}", userId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create user {Id}", userId);
            throw;
        }
        return userResponse.Resource;
    }
    
    public async Task AddCredits(string userId, int count, bool countResets = false)
    {
        var user = await GetUser(userId);
        if (user is null)
        {
            return;
        }

        if (countResets)
        {
            user.CreditResets++;
        }
        user.CreditsCount += count;
        var userResponse = await _container.UpsertItemAsync(user, new PartitionKey(User.PARTITION_KEY));
        logger.LogInformation("Added {CreditCount} credits to user {Id}", count, userId);
    }

    public async Task RemoveCredits(string userId, int count)
    {
        var user = await GetUser(userId);
        if (user is null)
        {
            return;
        }
        user.CreditsCount -= count;
        var userResponse = await _container.UpsertItemAsync(user, new PartitionKey(User.PARTITION_KEY));
        logger.LogInformation("User {Id} consumed {CreditCount} credits", userId, count);
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
                ConnectionMode = ConnectionMode.Gateway
            });
        try
        {
            _database ??= await _cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get or create database");
            throw;
        }

        try
        {
            _container ??= await _database.CreateContainerIfNotExistsAsync(cosmosConfig.UserContainerId, "/partitionKey", 400);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get or create container");
            throw;
        }
    }

    public async Task UpdateSelectedCompany(string userId, SupportedCompany? selectedTargetCompany)
    {
        var user = await GetUser(userId);
        if (user is null)
        {
            return;
        }
        user.LastSelectedCompany = selectedTargetCompany;
        var userResponse = await _container.UpsertItemAsync(user, new PartitionKey(User.PARTITION_KEY));
        logger.LogInformation("User {UserId} selected {@SelectedTargetCompany} as default export target", userId, selectedTargetCompany);
    }

    public async Task UpdateUser(User dbUser)
    {
        try
        {
            var userResponse = await _container.UpsertItemAsync(dbUser, new PartitionKey(User.PARTITION_KEY));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update user {Id}", dbUser.Id);
            throw;
        }
    }
}