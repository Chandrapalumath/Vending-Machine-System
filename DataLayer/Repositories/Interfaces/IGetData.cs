using DataLayer.Models;

namespace DataLayer.Repositories.Interfaces
{
    public interface IGetData<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<User?> GetByUserNameAsync(string userName);
    }
}
