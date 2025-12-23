using Backend.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;
using Backend.Model;

namespace Vending_Machine_System.Menus
{
    public class UserMenu
    {
        private User _currentUser;
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;

        public UserMenu(
            User currentUser,
            IItemService itemService,
            ITransactionService transactionService,
            IUserService userService)
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
            var items = await _itemService.GetAllItemsAsync();
            var selectedItems = new List<string>();
            var selectedPrices = new List<float>();
            var selectedQuantities = new List<int>();

            Console.Clear();
            Console.WriteLine("=== SHOPPING CART ===");

            while (true)
            {
                ShowAvailableItems(items);
                Console.Write("\nEnter item ID or 'exit': ");
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "exit") break;

                if (!int.TryParse(input, out var index) || index < 0 || index >= items.Count || items[index].Quantity <= 0)
                {
                    Console.WriteLine("Invalid selection or out of stock!");
                    continue;
                }

                var qty = InputHelper.PromptPositiveInt($"Quantity for {items[index].Name}: ");
                if (qty > items[index].Quantity)
                {
                    Console.WriteLine($"Only {items[index].Quantity} available!");
                    continue;
                }

                selectedItems.Add(items[index].Name);
                selectedPrices.Add(items[index].Price);
                selectedQuantities.Add(qty);
                Console.WriteLine($"Added {qty}   {items[index].Name}");
            }

            if (selectedItems.Count == 0)
            {
                Console.WriteLine("No items selected.");
                InputHelper.Pause();
                return;
            }

            var totalAmount = selectedItems.Select((name, i) => selectedPrices[i] * selectedQuantities[i]).Sum();
            Console.WriteLine($"\nTotal: ${totalAmount:F2}");

            if (totalAmount > _currentUser.Wallet)
            {
                Console.WriteLine($"Insufficient funds! Need: ${totalAmount:F2}, Have: ${_currentUser.Wallet:F2}");
                if (InputHelper.Confirm("Add money now?"))
                    await AddMoneyAsync();
                return;
            }

            try
            {
                var transaction = new Transaction(
                    _currentUser.UserName,
                    selectedItems.ToArray(),
                    selectedPrices.ToArray(),
                    selectedQuantities.ToArray(),
                    totalAmount);

                await _transactionService.AddTransactionAsync(transaction);

                _currentUser.Wallet -= totalAmount;
                await _userService.UpdateUserWalletAsync(_currentUser.UserName, _currentUser.Wallet);

                Console.WriteLine("Purchase successful! Thank you!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchase failed: {ex.Message}");
            }
            InputHelper.Pause();
        }

        private void ShowAvailableItems(List<Item> items)
        {
            Console.WriteLine("\nAvailable Items:");
            Console.WriteLine("{0,-4} {1,-20} {2,-8} {3,-8}", "ID", "Item", "Price", "Stock");
            Console.WriteLine(new string('-', 45));

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Quantity > 0)
                    Console.WriteLine("{0,-4} {1,-20} ${2,-7:F2} {3,-7}", i, items[i].Name, items[i].Price, items[i].Quantity);
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
            Console.WriteLine("🔑 === RESET PASSWORD ===");

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
