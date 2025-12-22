using Backend.Model;

namespace Backend.Interfaces
{
    public interface ITransactionService
    {
        Task AddTransactionAsync(Transaction transaction);
    }
}
