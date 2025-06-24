using automach_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using automach_backend.Dto.Review;
using automach_backend.Mappers;



namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IAccountRepository _accountRepository;

        public ReviewController(IReviewRepository reviewRepository, IAccountRepository accountRepository)
        {
            _reviewRepository = reviewRepository;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews.Select(r => r.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review.ToDto());
        }

        [HttpGet("game/{gameId}")]
        public async Task<IActionResult> GetByGameId(int gameId)
        {
            var reviews = await _reviewRepository.GetByGameIdAsync(gameId);
            return Ok(reviews.Select(r => r.ToDto()));
        }

        // Can fix 
        [HttpPost]
        [Route("{accountId}/{gameId}")]
        public async Task<IActionResult> Create([FromRoute] int accountId, [FromRoute] int gameId, [FromBody] CreateReviewRequestDto reviewDto)
        {
            if (!await _accountRepository.AccountExists(accountId))
            {
                return BadRequest("Account does not exist.");
            }

            var review = reviewDto.ToModel(accountId, gameId);
            await _reviewRepository.CreateAsync(review, accountId, gameId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequestDto reviewDto)
        {
            var updatedReview = await _reviewRepository.UpdateAsync(id, reviewDto);
            if (updatedReview == null)
            {
                return NotFound();
            }
            return Ok(updatedReview.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedReview = await _reviewRepository.DeleteAsync(id);
            if (deletedReview == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}