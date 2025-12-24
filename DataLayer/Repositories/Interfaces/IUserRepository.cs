using DataLayer.Models;

namespace DataLayer.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task SaveAllAsync(List<User> entities);
        Task<User?> GetByUserNameAsync(string userName);
    }
}
