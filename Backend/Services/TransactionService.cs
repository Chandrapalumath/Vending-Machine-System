using Backend.Interfaces;
using DataLayer.Repositories.Interfaces;
using DataLayer_Models = DataLayer.Models;
using Backend.Model;
using Backend.Exceptions;

namespace Backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAddData<DataLayer_Models.Transaction> _addTransactions;
        private readonly IGetData<DataLayer_Models.User> _getUsers;
        private readonly IGetData<DataLayer_Models.Item> _getItems;
        private readonly IGetData<DataLayer_Models.Transaction> _getTransactions;
        private const int DefaultTransactionCount = 5;

        public TransactionService( IAddData<DataLayer_Models.Transaction> addTransactions, IGetData<DataLayer_Models.User> getUsers, IGetData<DataLayer_Models.Item> getItems, IGetData<DataLayer_Models.Transaction> getTransactions)
        {
            _addTransactions = addTransactions;
            _getUsers = getUsers;
            _getItems = getItems;
            _getTransactions = getTransactions;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.UserName)) throw new UserNotFoundException("User name is required.");

            var users = await _getUsers.GetAllAsync();
            var user = users.SingleOrDefault(u =>
                u.UserName.Equals(transaction.UserName, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new UserNotFoundException("User not found.");

            var items = await _getItems.GetAllAsync();
            for (int i = 0; i < transaction.Items.Length; i++)
            {
                var itemName = transaction.Items[i];
                var item = items.FirstOrDefault(x =>
                    x.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

                if (item == null || item.Quantity < transaction.Quantities[i])
                    throw new ItemNotFoundException($"Item {itemName} not available in required quantity.");
            }

            var entity = ToEntity(transaction);
            await _addTransactions.AddAsync(entity);
        }

        private static DataLayer_Models.Transaction ToEntity(Transaction model)
        {
            return new DataLayer_Models.Transaction
            {
                UserName = model.UserName,
                ItemsCsv = string.Join("|", model.Items),
                PricesCsv = string.Join("|", model.Prices.Select(p => p.ToString())),
                QuantitiesCsv = string.Join("|", model.Quantities.Select(q => q.ToString())),
                TotalAmount = model.TotalAmount,
                TimeUtc = model.TimeUtc
            };
        }
        public async Task<List<Transaction>> GetUserTransactionsAsync(string userName, int maxCount = 5)
        {
            var transactions = await _getTransactions.GetAllAsync();
            var userTransactions = transactions
                .Where(t => t.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.TimeUtc)
                .Take(maxCount)
                .Select(ToModel)
                .ToList();
            return userTransactions;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(int maxCount = 5)
        {
            var transactions = await _getTransactions.GetAllAsync();
            return transactions
                .OrderByDescending(t => t.TimeUtc)
                .Take(maxCount)
                .Select(ToModel)
                .ToList();
        }
        private static Transaction ToModel(DataLayer_Models.Transaction entity)
        {
            var items = entity.ItemsCsv.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var pricesStr = entity.PricesCsv.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var quantitiesStr = entity.QuantitiesCsv.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var prices = new float[pricesStr.Length];
            var quantities = new int[quantitiesStr.Length];

            for (int i = 0; i < pricesStr.Length; i++)
                float.TryParse(pricesStr[i], out prices[i]);

            for (int i = 0; i < quantitiesStr.Length; i++)
                int.TryParse(quantitiesStr[i], out quantities[i]);

            return new Transaction(
                entity.UserName,
                items,
                prices,
                quantities,
                entity.TotalAmount)
            {
                TimeUtc = entity.TimeUtc
            };
        }
    }
}
