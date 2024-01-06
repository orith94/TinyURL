using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TinyURL.Data
{
    public class UrlMapping
    {
        [BsonId]
        [StringLength(10)]
        public string ShortUrl { get; set; }
        [Required]
        public string OriginalUrl { get; set; }

    }
}
