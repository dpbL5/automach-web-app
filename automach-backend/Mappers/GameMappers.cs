using automach_backend.Dto.Game;
using automach_backend.Models;

namespace automach_backend.Mappers
{
    public static class GameMappers
    {
        // Ket qua tra ve la 1 object GameDto
        public static GameDto ToDto(this Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price,
                GameInfo = game.GameInfo,
                ReleaseDate = game.ReleaseDate,
                Developer = game.Developer,
                IsFeatured = game.IsFeatured,
            };
        }

        public static Game ToModel(this CreateGameRequestDto gameDto)
        {
            return new Game
            {
                Title = gameDto.Title,
                Price = gameDto.Price,
                GameInfo = gameDto.GameInfo,
                ReleaseDate = gameDto.ReleaseDate,
                Developer = gameDto.Developer,
                IsFeatured = gameDto.IsFeatured
            };
        }
    }
}