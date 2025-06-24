using Microsoft.AspNetCore.Mvc;
using automach_backend.Mappers;
using automach_backend.Dto.Account;
using automach_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace automach_backend.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepo;
        private readonly ITransactionRepository transactionRepo;
        
        public AccountController(IAccountRepository accountRepo, ITransactionRepository transactionRepo)
        {
            this.accountRepo = accountRepo;
            this.transactionRepo = transactionRepo;
        }
        
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await accountRepo.GetAllAsync();
            return Ok(accounts.Select(a => a.ToDto()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // Only admin or account owner can access account details
            if (!User.IsInRole("admin") && id != GetCurrentUserId())
            {
                return Forbid();
            }
            
            var account = await accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account.ToDto());
        }

        [HttpGet("{id}/public")]
        public async Task<IActionResult> GetPublicInfo([FromRoute] int id)
        {
            var account = await accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            
            // Return only public information for display purposes
            return Ok(new { 
                id = account.Id,
                name = account.Name ?? account.Username ?? "Anonymous",
                username = account.Username
            });
        }

        // Xem cac giao dich theo ID nguoi dung
        [HttpGet("{id}/transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactions([FromRoute] int id)
        {
            // Only admin or account owner can access account transactions
            if (!User.IsInRole("admin") && id != GetCurrentUserId())
            {
                return Forbid();
            }
            
            var account = await accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var transactions = await transactionRepo.GetTransactionsByAccountIdAsync(id);
            return Ok(transactions.Select(t => t.ToDto()));
        }


        // Tao tai khoan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequestDto accountDto)
        {
            var account = accountDto.ToModel();
            await accountRepo.CreateAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAccountRequestDto accountDto)
        {
            // Only admin or account owner can update account
            if (!User.IsInRole("admin") && id != GetCurrentUserId())
            {
                return Forbid();
            }
            
            var account = await accountRepo.UpdateAsync(id, accountDto);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var accountModel = await accountRepo.DeleteAsync(id);

            if (accountModel == null)
            {
                return NotFound();
            }
            return NoContent();
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