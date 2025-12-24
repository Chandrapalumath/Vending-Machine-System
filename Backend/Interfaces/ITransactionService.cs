using Backend.Model;

namespace Backend.Interfaces
{
    public interface ITransactionService
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetUserTransactionsAsync(string userName);
        Task<List<Transaction>> GetAllTransactionsAsync();
    }
}
