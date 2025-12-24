using DataLayer.ApplicationConstant;
using DataLayer.Repositories.Interfaces;
using DataLayer.Models;

namespace DataLayer.Repositories.FileSystem
{
    public class FileTransactionRepository : FileRepositoryBase, ITransactionRepository
    {
        public FileTransactionRepository() : base(DataFilePaths.TransactionsFile) { }
        public async Task AddAsync(Transaction transaction)
        {
            var transactions = await GetAllAsync();
            transactions.Add(transaction);
            var lines = transactions.Select(ToLine).ToList();
            await WriteAllLinesAsync(lines);
        }
        public async Task<List<Transaction>> GetAllAsync()
        {
            var result = new List<Transaction>();
            var lines = await ReadAllLinesAsync();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var transaction = ParseTransaction(line);
                result.Add(transaction);
            }
            return result;
        }

        private static Transaction ParseTransaction(string line)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            var transaction = new Transaction { TimeUtc = DateTime.UtcNow };
            if (parts.Length >= 1) transaction.UserName = parts[0];
            if (parts.Length >= 2) transaction.ItemsCsv = parts[1];
            if (parts.Length >= 3) transaction.PricesCsv = parts[2];
            if (parts.Length >= 4) transaction.QuantitiesCsv = parts[3];
            if (parts.Length >= 5 && float.TryParse(parts[4], out var total))
                transaction.TotalAmount = total;
            if (parts.Length >= 6 && DateTime.TryParse(parts[5], out var dt))
                transaction.TimeUtc = dt;

            return transaction;
        }
        private static string ToLine(Transaction transaction) =>
            $"{transaction.UserName},{transaction.ItemsCsv},{transaction.PricesCsv},{transaction.QuantitiesCsv},{transaction.TotalAmount},{transaction.TimeUtc:o}";
    }
}
