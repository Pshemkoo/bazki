using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GamesDirectory.Models
{
    public class GameRating
    {
        [BsonElement("Rating")]
        public int Rating { get; set; }
        [BsonElement("UserName")]
        public string UserName { get; set; }
        [BsonElement("Reason")]
        public string Reason { get; set; }
        [BsonElement("AddedDate")]
        public DateTime AddedDate { get; set; }
    }
}