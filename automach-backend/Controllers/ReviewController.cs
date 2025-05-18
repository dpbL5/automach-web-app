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

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequestDto reviewDto)
        {
            var review = await _reviewRepository.CreateAsync(reviewDto.ToModel());
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequestDto reviewDto)
        {
            var updatedReview = await _reviewRepository.UpdateAsync(id, reviewDto);
            if (updatedReview == null)
            {
                return NotFound();
            }
            return Ok(updatedReview);
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