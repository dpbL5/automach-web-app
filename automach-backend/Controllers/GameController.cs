using automach_backend.Dto.Game;
using automach_backend.Helpers;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var games = await gameRepo.GetAllAsync(query);
            return Ok(games.Select(g => g.ToDto()));
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var query = new QueryObject { IsFeatured = true };
            var games = await gameRepo.GetAllAsync(query);
            return Ok(games.Select(g => g.ToDto()));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] string? tag)
        {
            var query = new QueryObject
            {
                Title = title,
                GameTag = tag
            };
            
            var games = await gameRepo.GetAllAsync(query);
            return Ok(games.Select(g => g.ToDto()));
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