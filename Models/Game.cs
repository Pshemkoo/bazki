using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace GamesDirectory.Models
{
    public class Game
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }
        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("AverageRating")]
        public double AverageRating { get; set; }
        [BsonElement("Ratings")]
        public List<GameRating> Ratings{ get; set; }
    }
}
