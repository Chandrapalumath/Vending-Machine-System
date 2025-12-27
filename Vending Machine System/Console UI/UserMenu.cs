using Backend.Interfaces;
using Backend.Model;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;
using Vending_Machine_System.ApplicationConstants;

namespace Vending_Machine_System.Menus
{
    public class UserMenu
    {
        private User _currentUser;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly UserShopping _buyItems;
        private readonly UserAccount _account;

        public UserMenu(User currentUser, IItemService itemService, ITransactionService transactionService, IUserService userService, UserShopping usershopping, UserAccount userAccount)
        {
            _currentUser = currentUser;
            _itemService = itemService;
            _userService = userService;
            _buyItems = usershopping;
            _account = userAccount;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{_currentUser.UserName,-20} Wallet: ${_currentUser.Wallet,-10:F2}  {DateTime.Now:HH:mm:ss}");
                ShowMenu();
                var choice = InputHelper.PromptInt("Choice", Constant.UserMenuMin, Constant.UserMenuMax);

                try
                {
                    switch ((UserMenuOption)choice)
                    {
                        case UserMenuOption.ShowItemList: await ShowItemListAsync(); break;
                        case UserMenuOption.BuyItem: await _buyItems.BuyItemsAsync(); break;
                        case UserMenuOption.AddMoney: await _account.AddMoneyAsync(_currentUser); break;
                        case UserMenuOption.TransactionHistory: await _account.ShowUserTransactionsAsync(_currentUser); break;
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
