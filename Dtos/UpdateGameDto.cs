using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDirectory.Dtos
{
    public class UpdateGameDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
