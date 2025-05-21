using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Dto.Review
{
    public class CreateReviewRequestDto
    {
        // public int Id { get; set; }
        // public int GameId { get; set; }
        // public int AccountId { get; set; }
        public string? Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        // public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}