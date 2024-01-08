using Newtonsoft.Json;

namespace ATAFurniture.Functions;

public class User
{
    public const string PARTITION_KEY = "Users";
    [JsonProperty("id")]
    public string Id { get; init; }
    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int CreditsCount { get; set; }

    public User()
    {
        
    }

    public User(string aadId, string name, string email, int credits)
    {
        PartitionKey = PARTITION_KEY;
        Id = aadId;
        Name = name;
        Email = email;
        CreditsCount = credits;
    }
    
    public void AddCredits(int credits)
    {
        CreditsCount += credits;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}