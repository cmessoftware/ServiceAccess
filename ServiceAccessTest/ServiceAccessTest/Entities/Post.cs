using ServiceAccess.Entities;
using System.Text.Json.Serialization;

namespace WebApiPrueba.Entities
{
    public class Post : ResponseBase<Post>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

}
