using Newtonsoft.Json;

namespace ATAFurniture.Functions;

public class User
{
    public const string PARTITION_KEY = "Users";
    [JsonProperty("id")]
    public string Id { get; init; }
    [JsonProperty("partitionKey")]
    public string PartitionKey { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string CreditsCount { get; private set; }

    public User()
    {
        
    }

    public User(string aadId, string name, string email, string credits)
    {
        PartitionKey = PARTITION_KEY;
        Id = aadId;
        Name = name;
        Email = email;
        CreditsCount = credits;
    }
    
    public void AddCredits(int credits)
    {
        var creditsCount = int.Parse(CreditsCount);
        creditsCount += credits;
        CreditsCount = creditsCount.ToString();
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}