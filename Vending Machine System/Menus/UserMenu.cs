using Backend.Interfaces;
using Backend.Model;
using System.Xml.Linq;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;

namespace Vending_Machine_System.Menus
{
    public class UserMenu
    {
        private User _currentUser;
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;

        public UserMenu(User currentUser, IItemService itemService, ITransactionService transactionService, IUserService userService)
        {
            _currentUser = currentUser;
            _itemService = itemService;
            _transactionService = transactionService;
            _userService = userService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{_currentUser.UserName,-20} Wallet: ${_currentUser.Wallet,-10:F2}  {DateTime.Now:HH:mm:ss}");
                ShowMenu();
                var choice = InputHelper.PromptInt("Choice", 1, (int)UserMenuOption.Exit);

                try
                {
                    switch ((UserMenuOption)choice)
                    {
                        case UserMenuOption.ShowItemList: await ShowItemListAsync(); break;
                        case UserMenuOption.BuyItem: await BuyItemsAsync(); break;
                        case UserMenuOption.AddMoney: await AddMoneyAsync(); break;
                        case UserMenuOption.TransactionHistory: await ShowUserTransactionsAsync(); break;
                        case UserMenuOption.ResetPassword: await ResetPasswordAsync(); break;
                        case UserMenuOption.Exit: return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    InputHelper.Pause();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\n1. Item List");
            Console.WriteLine("2. Buy Items");
            Console.WriteLine("3. Add Money");
            Console.WriteLine("4. My Transactions");
            Console.WriteLine("5. Reset Password");
            Console.WriteLine("6. Logout");
        }

        private async Task ShowItemListAsync()
        {
            var items = await _itemService.GetAllItemsAsync();
            Console.WriteLine("\n=== AVAILABLE ITEMS ===");
            Console.WriteLine("{0,-4} {1,-20} {2,-8} {3,-8}", "ID", "Item", "Price", "Stock");
            Console.WriteLine(new string('-', 45));

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Quantity > 0)
                    Console.WriteLine("{0,-4} {1,-20} ${2,-7:F2} {3,-7}", i, items[i].Name, items[i].Price, items[i].Quantity);
            }
            InputHelper.Pause();
        }
        private async Task BuyItemsAsync()
        {
            var chosenItems = new List<string>();
            var chosenPrices = new List<float>();
            var chosenQty = new List<int>();

            var cartQuantities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                var items = await _itemService.GetAllItemsAsync();

                Console.Clear();
                Console.WriteLine("Available items:");
                Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-8}", "Idx", "Item", "Price", "Qty");

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var alreadySelected = cartQuantities.GetValueOrDefault(item.Name, 0);
                    var available = item.Quantity - alreadySelected;

                    if (available > 0)
                    {
                        Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-8}",
                            i, item.Name, item.Price, available);
                    }
                }

                Console.Write("\nEnter item index to buy or 'exit': ");
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "exit")
                    break;

                if (!int.TryParse(input, out var index) || index < 0 || index >= items.Count)
                {
                    Console.WriteLine("Invalid index.");
                    InputHelper.Pause();
                    continue;
                }

                var selectedItem = items[index];
                var alreadyInCart = cartQuantities.GetValueOrDefault(selectedItem.Name, 0);
                var availableQty = selectedItem.Quantity - alreadyInCart;

                if (availableQty <= 0)
                {
                    Console.WriteLine("Item already fully selected.");
                    InputHelper.Pause();
                    continue;
                }

                var quantity = InputHelper.PromptPositiveInt(
                    $"Enter quantity for {selectedItem.Name} (Available: {availableQty}): ");

                if (quantity > availableQty)
                {
                    Console.WriteLine("Not enough stock available.");
                    InputHelper.Pause();
                    continue;
                }
                cartQuantities[selectedItem.Name] = alreadyInCart + quantity;

                chosenItems.Add(selectedItem.Name);
                chosenPrices.Add(selectedItem.Price);
                chosenQty.Add(quantity);

                Console.WriteLine($"Added {quantity} x {selectedItem.Name}");
                InputHelper.Pause();
            }

            if (chosenItems.Count == 0)
            {
                Console.WriteLine("No items selected.");
                InputHelper.Pause();
                return;
            }
            float totalAmount = 0;
            for (int i = 0; i < chosenItems.Count; i++)
                totalAmount += chosenPrices[i] * chosenQty[i];

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Total Amount: ${totalAmount:F2}");
                Console.WriteLine($"Wallet Balance: ${_currentUser.Wallet:F2}");

                if (totalAmount > _currentUser.Wallet)
                {
                    Console.WriteLine("Insufficient wallet balance.");
                    if (InputHelper.Confirm("Add money to wallet?"))
                    {
                        await AddMoneyAsync();
                        continue;
                    }

                    Console.WriteLine("Order cancelled.");
                    InputHelper.Pause();
                    return;
                }

                var finalItems = await _itemService.GetAllItemsAsync();
                bool conflict = false;

                foreach (var kvp in cartQuantities)
                {
                    var item = finalItems.FirstOrDefault(x =>
                        x.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));

                    if (item == null || item.Quantity < kvp.Value)
                    {
                        conflict = true;
                        break;
                    }
                }

                if (conflict)
                {
                    Console.WriteLine("Item quantity changed. Please try again.");
                    InputHelper.Pause();
                    return;
                }

                foreach (var kvp in cartQuantities)
                {
                    var item = finalItems.First(x =>
                        x.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));

                    item.Quantity -= kvp.Value;
                }

                foreach (var item in finalItems) { 
                    await _itemService.UpdateItemAsync(item.Name, null, null, item.Quantity); 
                }

                _currentUser.Wallet -= totalAmount;
                await _userService.UpdateUserWalletAsync(
                    _currentUser.UserName, _currentUser.Wallet);

                var transaction = new Transaction(
                    _currentUser.UserName,
                    chosenItems.ToArray(),
                    chosenPrices.ToArray(),
                    chosenQty.ToArray(),
                    totalAmount);

                await _transactionService.AddTransactionAsync(transaction);

                Console.WriteLine("Purchase successful! Items dispensed.");
                InputHelper.Pause();
                return;
            }
        }

        private async Task AddMoneyAsync()
        {
            var amount = InputHelper.PromptPositiveFloat("Amount to add: ");
            var newWallet = _currentUser.Wallet + amount;

            try
            {
                await _userService.UpdateUserWalletAsync(_currentUser.UserName, newWallet);
                _currentUser.Wallet = newWallet;
                Console.WriteLine($"Added ${amount:F2}! New balance: ${_currentUser.Wallet:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add money: {ex.Message}");
            }
            InputHelper.Pause();
        }

        private async Task ShowUserTransactionsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== TRANSACTIONS ===");
            var transactions = await _transactionService.GetUserTransactionsAsync(_currentUser.UserName, 5);

            if (!transactions.Any())
            {
                Console.WriteLine("No transactions found.");
                InputHelper.Pause();
                return;
            }

            Console.WriteLine(new string('═', 70));
            foreach (var txn in transactions)
            {
                Console.WriteLine($"${txn.TotalAmount,-8:F2}  {txn.TimeUtc:yyyy-MM-dd HH:mm}");
                for (int i = 0; i < txn.Items.Length; i++)
                {
                    Console.WriteLine($"   {txn.Items[i],-20}  {txn.Quantities[i],-3}   ${txn.Prices[i],-6:F2}");
                }
            }
            InputHelper.Pause();
        }

        private async Task ResetPasswordAsync()
        {
            Console.Clear();
            Console.WriteLine("=== RESET PASSWORD ===");

            var currentPassword = InputHelper.PromptPassword("Current password: ");
            var user = await _userService.ValidateUserAsync(_currentUser.UserName, currentPassword);

            if (user == null)
            {
                Console.WriteLine("Current password is incorrect!");
                InputHelper.Pause();
                return;
            }

            var newPassword = InputHelper.PromptPassword("New password: ");
            var confirmPassword = InputHelper.PromptPassword("Confirm password: ");

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("Passwords do not match!");
                InputHelper.Pause();
                return;
            }

            try
            {
                await _userService.UpdatePasswordAsync(_currentUser.UserName, newPassword);
                Console.WriteLine("Password updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password update failed: {ex.Message}");
            }
            InputHelper.Pause();
        }

    }
}
