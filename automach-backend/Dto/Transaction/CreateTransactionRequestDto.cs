using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Dto.Transaction
{
    public class CreateTransactionRequestDto
    {
        public int AccountId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty;
        public float TotalPrice { get; set; }
    }
}