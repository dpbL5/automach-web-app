using Microsoft.AspNetCore.Mvc;
using automach_backend.Interfaces;
using automach_backend.Dto.Game;
using automach_backend.Mappers;
using automach_backend.Dto.GameOwned;

namespace automach_backend.Controllers
{
    [Route("api/accounts/{accountId}/owned-games")]
    [ApiController]
    public class GameOwnedController : ControllerBase
    {
        private readonly IGameOwnedRepository _gameOwnedRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IGameRepository _gameRepository;

        public GameOwnedController(
            IGameOwnedRepository gameOwnedRepository,
            IAccountRepository accountRepository,
            IGameRepository gameRepository)
        {
            _gameOwnedRepository = gameOwnedRepository;
            _accountRepository = accountRepository;
            _gameRepository = gameRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnedGames(int accountId)
        {
            // Verify account exists
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return NotFound($"Account with ID {accountId} not found");
            }

            var games = await _gameOwnedRepository.GetOwnedGamesByAccountIdAsync(accountId);
            return Ok(games.Select(g => g.ToDto()));
        }

        [HttpPost("{gameId}")]
        public async Task<IActionResult> AddGameToAccount(int accountId, int gameId)
        {
            // Verify account exists
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return NotFound($"Account with ID {accountId} not found");
            }

            // Verify game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found");
            }

            var gameOwned = await _gameOwnedRepository.AddGameToAccountAsync(accountId, gameId);
            return Ok();
        }
    }
} 