using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoApi.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("publishedAt")]
    public DateTime PublishedAt { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];
}