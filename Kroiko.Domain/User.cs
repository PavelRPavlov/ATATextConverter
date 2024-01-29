using Kroiko.Domain.CellsExtracting;
using Newtonsoft.Json;

namespace Kroiko.Domain;

public class User
{
    public const string PARTITION_KEY = "Users";
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }
    public int CreditsCount { get; set; }
    public int CreditResets { get; set; }
    public SupportedCompany? LastSelectedCompany { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string CompanyName { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}