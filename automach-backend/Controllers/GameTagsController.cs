using Microsoft.AspNetCore.Mvc;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using automach_backend.Dto.GameTag;

namespace automach_backend.Controllers
{
    [Route("api/games/{gameId}/tags")]
    [ApiController]
    public class GameTagsController : ControllerBase
    {
        private readonly IGameTagRepository _gameTagRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ITagRepository _tagRepository;

        public GameTagsController(
            IGameTagRepository gameTagRepository,
            IGameRepository gameRepository,
            ITagRepository tagRepository)
        {
            _gameTagRepository = gameTagRepository;
            _gameRepository = gameRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameTags(int gameId)
        {
            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            var tags = await _gameTagRepository.GetTagsByGameIdAsync(gameId);
            return Ok(tags.Select(t => t.ToDto()));
        }

        [HttpPost]
        public async Task<IActionResult> AddTagToGame(int gameId, [FromBody] AddTagRequestDto requestDto)
        {
            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            // Verify tag exists
            var tag = await _tagRepository.GetByIdAsync(requestDto.TagId);
            if (tag == null)
            {
                return NotFound($"Tag with ID {requestDto.TagId} not found");
            }

            var gameTag = await _gameTagRepository.AddTagToGameAsync(gameId, requestDto.TagId);
            return Ok();
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> RemoveTagFromGame(int gameId, int tagId)
        {
            var result = await _gameTagRepository.RemoveTagFromGameAsync(gameId, tagId);
            if (result == null)
            {
                return NotFound($"Tag with ID {tagId} is not associated with game ID {gameId}");
            }

            return NoContent();
        }
    }
} 