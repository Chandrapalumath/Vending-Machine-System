using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Model;
using DataLayer.Repositories.Interfaces;
using DataLayer_Model = DataLayer.Models;

namespace Backend.Services
{
    public class ItemService : IItemService
    {
        private readonly IGetData<DataLayer_Model.Item> _getItems;
        private readonly IAddData<DataLayer_Model.Item> _addItems;
        private readonly ISaveAllData<DataLayer_Model.Item> _saveItems;

        public ItemService(IGetData<DataLayer_Model.Item> getItems, IAddData<DataLayer_Model.Item> addItems, ISaveAllData<DataLayer_Model.Item> saveItems)
        {
            _getItems = getItems;
            _addItems = addItems;
            _saveItems = saveItems;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            var entities = await _getItems.GetAllAsync();
            return entities.Select(ToModel).ToList();
        }

        public async Task<Item?> GetItemByNameAsync(string name)
        {
            var items = await _getItems.GetAllAsync();
            var item = items.SingleOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return null;
            return ToModel(item);
        }

        public async Task AddItemAsync(string name, float price, int quantity)
        {
            if (!IsValidItemName(name)) throw new ItemValidationException("Invalid item name.");
            if (price < 0) throw new NegativeValueException("Price");
            if (quantity < 0) throw new NegativeValueException("Quantity");

            var items = await _getItems.GetAllAsync();
            if (items.Any(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ItemAlreadyExistsException(name);

            var item = new DataLayer_Model.Item { Name = name.Trim(), Price = price, Quantity = quantity };
            await _addItems.AddAsync(item);
        }

        // Check hasvalue and value in nullables
        public async Task UpdateItemAsync(string name, string? newName, float? newPrice, int? newQuantity)
        {
            var items = await _getItems.GetAllAsync();
            var item = items.SingleOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
                throw new ItemNotFoundException("Item not found");

            if (newName != null)
            {
                if (!IsValidItemName(newName))
                    throw new ItemValidationException("Invalid new item name.");
                item.Name = newName.Trim();
            }
            if (newPrice.HasValue && newPrice.Value < 0)
                throw new NegativeValueException("Price");
            if (newPrice.HasValue) item.Price = newPrice.Value;
            if (newQuantity.HasValue && newQuantity.Value < 0)
                throw new NegativeValueException("Quantity");
            if (newQuantity.HasValue) item.Quantity = newQuantity.Value;

            await _saveItems.SaveAllAsync(items);
        }

        public async Task RemoveItemAsync(string name)
        {
            var items = await _getItems.GetAllAsync();
            items.RemoveAll(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            await _saveItems.SaveAllAsync(items);
        }

        private static Item ToModel(DataLayer_Model.Item entity) =>
            new Item(entity.Name, entity.Price, entity.Quantity);

        private static bool IsValidItemName(string name)
        {
            name = name.Trim();
            return !string.IsNullOrWhiteSpace(name) &&
                   !name.Contains(',') && !name.Contains('|');
        }
    }
}
