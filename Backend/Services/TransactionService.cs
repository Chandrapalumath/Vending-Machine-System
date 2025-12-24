using Backend.Interfaces;
using DataLayer.Repositories.Interfaces;
using DataLayer_Models = DataLayer.Models;
using Backend.Model;
using Backend.Exceptions;
using Backend.ApplicationConstants;

namespace Backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.UserName)) throw new UserNotFoundException("User name is required.");
            var entity = ToModel(transaction);
            await _transactionRepository.AddAsync(entity);
        }

        private static DataLayer_Models.Transaction ToModel(Transaction model)
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
        public async Task<List<Transaction>> GetUserTransactionsAsync(string userName)
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var userTransactions = transactions
                .Where(t => t.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.TimeUtc)
                .Take(TransactionRules.DefaultTransactionCount)
                .Select(ToModel)
                .ToList();
            return userTransactions;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions
                .OrderByDescending(t => t.TimeUtc)
                .Take(TransactionRules.DefaultTransactionCount)
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
