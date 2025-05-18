using Microsoft.AspNetCore.Mvc;
using automach_backend.Mappers;
using automach_backend.Dto.Account;
using automach_backend.Interfaces;

namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepo;
        public AccountController(IAccountRepository accountRepo)
        {
            this.accountRepo = accountRepo;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await accountRepo.GetAllAsync();
            return Ok(accounts.Select(a => a.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var account = await accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequestDto accountDto)
        {
            var account = accountDto.ToModel();
            await accountRepo.CreateAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAccountRequestDto accountDto)
        {
            var account = await accountRepo.UpdateAsync(id, accountDto);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var accountModel = await accountRepo.DeleteAsync(id);

            if (accountModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}