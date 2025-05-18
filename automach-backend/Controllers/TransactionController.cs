using automach_backend.Dto.Transaction;
using automach_backend.Interfaces;
using automach_backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return Ok(transactions.Select(t => t.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction.ToDto());
        }

        [HttpPost("{accountId}")]
        public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto, [FromRoute] int accountId)
        {
            // Check if the account exists
            if (!await _accountRepository.AccountExists(accountId))
            {
                return BadRequest("Account does not exist.");
            }

            var transaction = transactionDto.ToModel(accountId);
            await _transactionRepository.CreateAsync(transaction);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction.ToDto());
        }
    }
}