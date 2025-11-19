using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookstoreApplication.Models.Mongo
{
    public class ComicIssueDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("comicVineIssueId")]
        public int ComicVineIssueId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("coverUrl")]
        public string CoverUrl { get; set; } = null!;

        [BsonElement("releaseDate")]
        public string ReleaseDate { get; set; } = null!;

        [BsonElement("issueNumber")]
        public string IssueNumber { get; set; } = null!;

        [BsonElement("pageCount")]
        public int PageCount { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}

