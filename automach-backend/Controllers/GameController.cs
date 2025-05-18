using automach_backend.Data;
using automach_backend.Dto.Game;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Mvc;


namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly DataContext context;

        public GameController(DataContext context)
        {
            this.context = context;
        }


        [HttpGet("get_list")]
        public IActionResult GetList()
        {
            var games = context.Games.ToList()
             .Select(g => g.ToDto());
            return Ok(games);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var game = context.Games.Find(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateGameRequestDto gameDto)
        {
            var gameModel = gameDto.ToModel();
            context.Games.Add(gameModel);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = gameModel.Id }, gameDto);
        }
    }
}