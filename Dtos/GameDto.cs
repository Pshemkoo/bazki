using GamesDirectory.Models;
using System.Collections.Generic;

namespace GamesDirectory.Dtos
{
    public class GameDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public List<GameRating> Ratings { get; set; }
    }
}
