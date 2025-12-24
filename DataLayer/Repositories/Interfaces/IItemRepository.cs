using DataLayer.Models;

namespace DataLayer.Repositories.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task SaveAllAsync(List<Item> entities);
        Task<Item?> GetByNameAsync(string name);
    }
}
