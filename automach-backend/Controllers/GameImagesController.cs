using Microsoft.AspNetCore.Mvc;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using automach_backend.Dto.ImageUrl;
using Microsoft.AspNetCore.Authorization;

namespace automach_backend.Controllers
{
    [Route("api/games/{gameId}/images")]
    [ApiController]
    public class GameImagesController : ControllerBase
    {
        private readonly IImageUrlRepository _imageUrlRepository;
        private readonly IGameRepository _gameRepository;

        public GameImagesController(
            IImageUrlRepository imageUrlRepository,
            IGameRepository gameRepository)
        {
            _imageUrlRepository = imageUrlRepository;
            _gameRepository = gameRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameImages(int gameId)
        {
            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            var images = await _imageUrlRepository.GetImagesByGameIdAsync(gameId);
            return Ok(images.Select(i => i.ToDto()));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddImageToGame(int gameId, [FromBody] CreateImageUrlRequestDto imageDto)
        {
            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            var imageUrl = imageDto.ToModel(gameId);
            var createdImage = await _imageUrlRepository.CreateAsync(imageUrl);
            return CreatedAtAction(nameof(GetGameImages), new { gameId }, createdImage.ToDto());
        }

        [HttpDelete("{imageId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteImage(int gameId, int imageId)
        {
            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            var image = await _imageUrlRepository.GetByIdAsync(imageId);
            if (image == null || image.GameId != gameId)
            {
                return NotFound($"Image with ID {imageId} not found for game {gameId}");
            }

            var deletedImage = await _imageUrlRepository.DeleteAsync(imageId);
            return NoContent();
        }
    }
} 