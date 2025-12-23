using Backend.Model;

namespace Backend.Interfaces
{
    public interface ITransactionService
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetUserTransactionsAsync(string userName, int transactionNeeded);
        Task<List<Transaction>> GetAllTransactionsAsync(int transactionNeeded);
    }
}
