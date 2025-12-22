using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public float Wallet { get; set; }
    }
}

public class FileHandler : IFileHandler
{
    public FileHandler()
    {
        if (!Directory.Exists(Config.DataFolder)) Directory.CreateDirectory(Config.DataFolder);
        var filesPath = new List<string>() { Config.UsersFile, Config.ItemsFile, Config.TransactionsFile };
        foreach (var filepath in filesPath)
        {
            if (!File.Exists(filepath)) File.WriteAllText(filepath, string.Empty);
        }
    }

    // USERS
    public List<UserModel> GetAllUsers()
    {
        var list = new List<UserModel>();
        try
        {
            if (!File.Exists(Config.UsersFile)) return list;
            var lines = File.ReadAllLines(Config.UsersFile);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                list.Add(UserModel.FromLine(line));
            }
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
        return list;
    }

    public void SaveAllUsers(List<UserModel> users)
    {
        try
        {
            var lines = new List<string>();
            foreach (var user in users) lines.Add(user.ToString());
            File.WriteAllLines(Config.UsersFile, lines);
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
    }

    public void AddUser(UserModel user)
    {
        try
        {
            File.AppendAllText(Config.UsersFile, "\n" + user.ToString());
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
    }

    // ITEMS
    public List<ItemModel> GetAllItems()
    {
        var list = new List<ItemModel>();
        try
        {
            if (!File.Exists(Config.ItemsFile)) return list;
            var lines = File.ReadAllLines(Config.ItemsFile);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                list.Add(ItemModel.FromLine(line));
            }
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
        return list;
    }

    public void SaveAllItems(List<ItemModel> items)
    {
        try
        {
            var lines = new List<string>();
            foreach (var item in items) lines.Add(item.ToString());
            File.WriteAllLines(Config.ItemsFile, lines);
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
    }

    public void AddItem(ItemModel item)
    {
        try
        {
            File.AppendAllText(Config.ItemsFile, "\n" + item.ToString());
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
    }

    // TRANSACTIONS
    public List<TransactionModel> GetAllTransactions()
    {
        var list = new List<TransactionModel>();
        try
        {
            if (!File.Exists(Config.TransactionsFile)) return list;
            var lines = File.ReadAllLines(Config.TransactionsFile);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                list.Add(TransactionModel.FromLine(line));
            }
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
        return list;
    }

    public void AddTransaction(TransactionModel transaction)
    {
        try
        {
            File.AppendAllText(Config.TransactionsFile, "\n" + transaction.ToString());
        }
        catch
        {
            Console.WriteLine("Error while working with file");
        }
    }
}