using automach_backend.Dto.Game;
using automach_backend.Helpers;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace automach_backend.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository gameRepo;

        public GameController(IGameRepository gameRepo)
        {
            this.gameRepo = gameRepo;
        }

        // Get all games with filtering and pagination
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query, [FromQuery] List<string>? gameTags = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Handle multiple tags from query string
            if (gameTags != null && gameTags.Any())
            {
                query.GameTags = gameTags;
            }

            var (games, totalCount) = await gameRepo.GetAllAsync(query);
            
            var paginatedResponse = new PaginationResponse<GameDto>
            {
                Items = games.Select(g => g.ToDto()).ToList(),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
            
            return Ok(paginatedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var game = await gameRepo.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            var game = gameDto.ToModel();
            await gameRepo.CreateAsync(game);
            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateGameRequestDto gameDto)
        {
            var game = await gameRepo.UpdateAsync(id, gameDto);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var game = await gameRepo.DeleteAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game.ToDto());
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetGameStats()
        {
            var (allGames, totalCount) = await gameRepo.GetAllAsync(new QueryObject { PageSize = int.MaxValue });
            var featuredCount = allGames.Count(g => g.IsFeatured);
            
            var stats = new
            {
                TotalGames = totalCount,
                FeaturedGames = featuredCount,
                NormalGames = totalCount - featuredCount
            };
            
            return Ok(stats);
        }
        
        // Temporary endpoint to test setting featured games - REMOVE IN PRODUCTION
        [HttpPost("test-featured/{id}")]
        public async Task<IActionResult> TestSetFeatured([FromRoute] int id, [FromQuery] bool featured = true)
        {
            var game = await gameRepo.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            
            var updateDto = new UpdateGameRequestDto
            {
                Title = game.Title,
                GameInfo = game.GameInfo,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                Developer = game.Developer,
                IsFeatured = featured
            };
            
            var updatedGame = await gameRepo.UpdateAsync(id, updateDto);
            return Ok(updatedGame?.ToDto());
        }
    }
}