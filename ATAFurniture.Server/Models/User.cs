using Newtonsoft.Json;

namespace ATAFurniture.Server.Models;

public class User
{
    public const string PARTITION_KEY = "Users";
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }
    public int CreditsCount { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public void AddCredits(int creditsToAdd)
    {
        CreditsCount += creditsToAdd;
    }
}