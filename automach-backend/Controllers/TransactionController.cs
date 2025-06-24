using automach_backend.Dto.Transaction;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace automach_backend.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        public TransactionController(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return Ok(transactions.Select(t => t.ToDto()));
        }

        [HttpGet("revenue-data")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetRevenueData()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            
            var revenueData = transactions
                .GroupBy(t => new { Year = t.CreatedAt.Year, Month = t.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    TotalRevenue = g.Sum(t => t.TotalPrice),
                    TransactionCount = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();
            
            return Ok(revenueData);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            
            // Check if user is admin or owns this transaction
            if (User.IsInRole("admin") || transaction.AccountId == GetCurrentUserId())
            {
                return Ok(transaction.ToDto());
            }
            
            return Forbid();
        }

        [HttpPost("{accountId}")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto, [FromRoute] int accountId)
        {
            // Check if the account exists
            if (!await _accountRepository.AccountExists(accountId))
            {
                return BadRequest("Account does not exist.");
            }
            
            // Only admin or the account owner can create transactions
            if (!User.IsInRole("admin") && accountId != GetCurrentUserId())
            {
                return Forbid();
            }

            var transaction = transactionDto.ToModel(accountId);
            await _transactionRepository.CreateAsync(transaction);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction.ToDto());
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}