using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Dto.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public float TotalPrice { get; set; }
        public List<int> GameIds { get; set; } = new List<int>();

    }
}