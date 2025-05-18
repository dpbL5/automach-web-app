using automach_backend.Dto.Game;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository gameRepo;

        public GameController(IGameRepository gameRepo)
        {
            this.gameRepo = gameRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await gameRepo.GetAllAsync();
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
        // [Authorize] // Assuming you want to restrict this action to authenticated users
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            var game = gameDto.ToModel();
            await gameRepo.CreateAsync(game);
            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        // [Authorize] // Assuming you want to restrict this action to authenticated users
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
        // [Authorize]
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