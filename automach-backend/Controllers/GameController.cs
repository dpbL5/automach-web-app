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
    }
}