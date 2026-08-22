using ToMainApi.Common;
using ToMainApi.Models.Dtos.Payments;

namespace ToMainApi.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResponse<List<TransactionDto>>> GetAllTransactions();
        Task<ServiceResponse<bool>> Credit(CreditRequest request);
        Task<ServiceResponse<bool>> Debit(DebitRequest request);
    }
}
