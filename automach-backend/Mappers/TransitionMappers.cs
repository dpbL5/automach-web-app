using automach_backend.Models;
using automach_backend.Dto.Transaction;

namespace automach_backend.Mappers
{
    public static class TransitionMappers
    {
        public static TransactionDto ToDto(this Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                CreatedAt = transaction.CreatedAt,
                PaymentMethod = transaction.PaymentMethod,
                TotalPrice = transaction.TotalPrice
            };
        }
        public static Transaction ToModel(this TransactionDto dto, int accountId)
        {
            return new Transaction
            {
                AccountId = accountId,
                CreatedAt = dto.CreatedAt,
                PaymentMethod = dto.PaymentMethod,
                TotalPrice = dto.TotalPrice
            };
        }
    }
}