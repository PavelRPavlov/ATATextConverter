using Newtonsoft.Json;

namespace Models
{
    public class User
    {
        public const string PARTITION_KEY = "Users";
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public int CreditsCount { get; init; }

        public User(string aadId, string name, string email, int credits = 0)
        {
            PartitionKey = PARTITION_KEY;
            Id = aadId;
            Name = name;
            Email = email;
            CreditsCount = credits;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}