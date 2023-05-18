using ServiceAccess.Entities;
using System.Text.Json.Serialization;

namespace ServiceAccessTest.Entiities
{
    public class User : ResponseBase<User>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("UserId")] 
        public int UserId { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("due_on")]
        public DateTime DueOn { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
