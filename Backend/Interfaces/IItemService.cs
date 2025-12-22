using Backend.Model;

namespace Backend.Interfaces
{
    public interface IItemService
    {
        Task<List<Item>> GetAllItemsAsync();
        Task<Item?> GetItemByNameAsync(string name);
        Task AddItemAsync(string name, float price, int quantity);
        Task UpdateItemAsync(string name, string? newName, float? newPrice, int? newQuantity);
        Task RemoveItemAsync(string name);
    }
}
