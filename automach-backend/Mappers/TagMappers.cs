using automach_backend.Dto.Tag;
using automach_backend.Models;

namespace automach_backend.Mappers
{
    public static class TagMappers
    {
        public static TagDto ToDto(this Tag tagModel)
        {
            return new TagDto
            {
                Id = tagModel.Id,
                Title = tagModel.Title
            };
        }

        public static Tag ToModel(this CreateTagRequestDto tagDto)
        {
            return new Tag
            {
                Title = tagDto.Title
            };
        }
    }
} 