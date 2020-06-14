using GamesDirectory.Dtos;
using GamesDirectory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDirectory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GamesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Seeding")]
        public async Task<IActionResult> Seeding()
        {
            try
            {
                var mongodb = GetMongoCollection();
                await mongodb.DeleteManyAsync(new BsonDocument { });
                await mongodb.Indexes.DropAllAsync();

                var lmyRating = new GameRating();
                lmyRating.Rating = 5;
                lmyRating.UserName = "Kamil";
                lmyRating.Reason = "Mnie League of Legends absolutnie oczarowało. To świetna, dopracowana gra multiplayerowa i każdy, komu podobało się Diablo, lubi orać expy i killować monstery (oraz innych graczy) powinien być zachwycony.";
                lmyRating.AddedDate = DateTime.Now;
                var defaultGameRating = new List<GameRating>();
                defaultGameRating.Add(lmyRating);

                var vmyRating = new GameRating();
                vmyRating.Rating = 5;
                vmyRating.UserName = "Przemyslaw";
                vmyRating.Reason = "Valorant bezkompromisowo uderza tam, gdzie powinien - w serca graczy tęskniących za wielogodzinnymi sesjami, kończonymi satysfakcją z rozegranych meczów.";
                vmyRating.AddedDate = DateTime.Now;
                var defaultGameRating2 = new List<GameRating>();
                defaultGameRating2.Add(vmyRating);

                var leaugeOfLegends = new Game
                {
                    Id = new ObjectId(),
                    ImageUrl = "https://static.antyweb.pl/uploads/2013/09/League-of-legends-Champions-1420x670.jpg",
                    Description = "Sieciowa gra komputerowa z gatunku multiplayer online battle arena. Powstała na bazie modyfikacji Defense of the Ancients do Warcraft III: The Frozen Throne. Została wyprodukowana i wydana przez studio Riot Games, początkowo tylko dla systemu Windows.",
                    Name = "Leauge of Legends",
                    Ratings = defaultGameRating,
                    AverageRating = 5
                };

                var valorant = new Game
                {
                    Id = new ObjectId(),
                    ImageUrl = "https://esportlife.pl/wp-content/uploads/2020/04/valo-1024x576.jpg",
                    Description = "Sieciowa strzelanka pierwszoosobowa oparta na modelu free-to-play. Zapowiedziana 15 października 2019 jako „Project A”, ostatecznie ukazała się pod obecną nazwą 2 marca 2020. Zamknięta beta rozpoczęła się 7 kwietnia 2020 w rejonach Europy i Ameryki Północnej. Premiera odbyła się 2 czerwca 2020.",
                    Name = "Valorant",
                    Ratings = defaultGameRating2,
                    AverageRating = 5
                };

                await mongodb.InsertOneAsync(leaugeOfLegends);
                await mongodb.InsertOneAsync(valorant);

                return Ok("Seeding successfully completed.");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mongodb = GetMongoCollection();
                var documents = await mongodb.AsQueryable().ToListAsync();
                var games = new List<GameDto>();
                foreach (var document in documents)
                {
                    games.Add(MapToDto(document));
                }

                return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            try
            {
                var mongodb = GetMongoCollection();
                var document = await mongodb.Find(g => g.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();

                return Ok(MapToDto(document));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]string id)
        {
            try
            {
                var mongodb = GetMongoCollection();
                await mongodb.DeleteOneAsync(g => g.Id == ObjectId.Parse(id));

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]NewGameDto newGameDto)
        {
            try
            {
                var mongodb = GetMongoCollection();
                var game = new Game
                {
                    Id = new ObjectId(),
                    ImageUrl = newGameDto.ImageUrl,
                    Description = newGameDto.Description,
                    Name = newGameDto.Name,
                    Ratings = new List<GameRating>(),
                    AverageRating = 0
                };

                await mongodb.InsertOneAsync(game);

                return Ok(MapToDto(game));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]UpdateGameDto updateGameDto, [FromRoute]string id)
        {
            try
            {
                var mongodb = GetMongoCollection();
                var document = await mongodb.Find(g => g.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();

                document.Name = updateGameDto.Name;
                document.Description = updateGameDto.Description;
                document.ImageUrl = updateGameDto.ImageUrl;

                await mongodb.ReplaceOneAsync(g => g.Id == ObjectId.Parse(id), document);

                return Ok(MapToDto(document));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("Rate/{id}")]
        public async Task<IActionResult> Rate([FromRoute]string id, [FromBody]RateDto rateDto)
        {
            try
            {
                var mongodb = GetMongoCollection();
                var document = await mongodb.Find(g => g.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();

                document.Ratings.Add(new GameRating
                {
                    Rating = rateDto.Rating,
                    UserName = rateDto.UserName,
                    Reason = rateDto.Reason,
                    AddedDate = DateTime.Now
                });

                var sumOfRatings = document.Ratings.Sum(r => r.Rating);
                var count = document.Ratings.Count;

                document.AverageRating = Math.Round((double)sumOfRatings / count, 2);

                await mongodb.ReplaceOneAsync(g => g.Id == ObjectId.Parse(id), document);

                return Ok(MapToDto(document));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private IMongoCollection<Game> GetMongoCollection()
        {
            var client = new MongoClient(_configuration["MongoDBAtlasConnectionString"]);
            var database = client.GetDatabase("GamesDirectory");
            var collection = database.GetCollection<Game>("inventory");
            return collection;
        }

        private GameDto MapToDto(Game game)
        {
            return new GameDto
            {
                Id = game.Id.ToString(),
                Name = game.Name,
                Description = game.Description,
                ImageUrl = game.ImageUrl,
                Ratings = game.Ratings,
                AverageRating = game.AverageRating
            };
        }
    }
}
