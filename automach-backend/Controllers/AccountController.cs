using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace automach_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Account>))]
        public IActionResult GetAccounts()
        {
            var accounts = accountRepository.GetAccounts();

            // Validate the model state
            // Kh valid thì trả về BadRequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(accounts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Account))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAccount(int id)
        {
            if (!accountRepository.AccountExists(id))
            {
                return NotFound();
            }

            var account = accountRepository.GetAccount(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(account);
        }
    }

}