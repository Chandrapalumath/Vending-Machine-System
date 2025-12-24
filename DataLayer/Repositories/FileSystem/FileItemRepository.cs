using DataLayer.ApplicationConstant;
using DataLayer.Repositories.Interfaces;
using DataLayer.Models;

namespace DataLayer.Repositories.FileSystem
{
    public class FileItemRepository : FileRepositoryBase, IItemRepository
    {
        public FileItemRepository() : base(DataFilePaths.ItemsFile) { }
        public async Task<List<Item>> GetAllAsync()
        {
            var result = new List<Item>();
            var lines = await ReadAllLinesAsync();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var item = ParseItem(line);
                result.Add(item);
            }
            return result;
        }
        public async Task AddAsync(Item item)
        {
            var items = await GetAllAsync();
            items.Add(item);
            await SaveAllAsync(items);
        }
        public async Task SaveAllAsync(List<Item> items)
        {
            var lines = items.Select(ToLine).ToList();
            await WriteAllLinesAsync(lines);
        }
        public async Task<Item?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var items = await GetAllAsync();
            return items.SingleOrDefault(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        internal async Task UpdateAsync(Item item)
        {
            var items = await GetAllAsync();
            var index = items.FindIndex(i =>
                i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                items[index] = item;
                await SaveAllAsync(items);
            }
        }

        internal async Task DeleteAsync(string name)
        {
            var items = await GetAllAsync();
            items.RemoveAll(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            await SaveAllAsync(items);
        }

        private static Item ParseItem(string line)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            var item = new Item();
            if (parts.Length >= 1) item.Name = parts[0];
            if (parts.Length >= 2 && float.TryParse(parts[1], out var price))
                item.Price = price;
            if (parts.Length >= 3 && int.TryParse(parts[2], out var qty))
                item.Quantity = qty;

            return item;
        }

        private static string ToLine(Item item) =>
            $"{item.Name},{item.Price},{item.Quantity}";
    }
}
