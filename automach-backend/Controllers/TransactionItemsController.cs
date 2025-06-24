using Microsoft.AspNetCore.Mvc;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;
using automach_backend.Data;
using Microsoft.AspNetCore.Authorization;

namespace automach_backend.Controllers
{
    [Route("api/transactionitems")]
    [ApiController]
    public class TransactionItemsController : ControllerBase
    {
        private readonly DataContext _context;

        public TransactionItemsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.TransactionItems
                .Include(ti => ti.Game)
                .Include(ti => ti.Transaction)
                .ToListAsync();
                
            return Ok(items.Select(ti => new
            {
                ti.GameId,
                ti.TransactionId,
                Game = ti.Game,
                Transaction = ti.Transaction
            }));
        }

        [HttpGet("transaction/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> GetByTransactionId(int transactionId)
        {
            var items = await _context.TransactionItems
                .Where(ti => ti.TransactionId == transactionId)
                .Include(ti => ti.Game)
                    .ThenInclude(g => g.ImageUrls)
                .ToListAsync();
                
            return Ok(items.Select(ti => new
            {
                ti.GameId,
                ti.TransactionId,
                Game = ti.Game
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionItem item)
        {
            _context.TransactionItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }
    }
} 