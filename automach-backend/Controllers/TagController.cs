using Microsoft.AspNetCore.Mvc;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using automach_backend.Dto.Tag;
using Microsoft.AspNetCore.Authorization;

namespace automach_backend.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagRepository.GetAllAsync();
            return Ok(tags.Select(t => t.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateTagRequestDto tagDto)
        {
            var tag = tagDto.ToModel();
            await _tagRepository.CreateAsync(tag);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTagRequestDto tagDto)
        {
            var tagModel = tagDto.ToModel();
            var updatedTag = await _tagRepository.UpdateAsync(id, tagModel);
            if (updatedTag == null)
            {
                return NotFound();
            }
            return Ok(updatedTag.ToDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _tagRepository.DeleteAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 