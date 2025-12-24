using Backend.Interfaces;
using Backend.Model;
using Vending_Machine_System.Helpers;

namespace Vending_Machine_System.Menus
{
    public class UserAccount
    {
        private readonly User _currentUser;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        public UserAccount(ITransactionService transactionService, IUserService userService, User user)
        {
            _transactionService = transactionService;
            _userService = userService;
            _currentUser = user;
        }
        public async Task ShowUserTransactionsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== TRANSACTIONS ===");
            var transactions = await _transactionService.GetUserTransactionsAsync(_currentUser.UserName);

            if (!transactions.Any())
            {
                Console.WriteLine("No transactions found.");
                InputHelper.Pause();
                return;
            }

            Console.WriteLine(new string('═', 70));
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"${transaction.TotalAmount,-8:F2}  {transaction.TimeUtc:yyyy-MM-dd HH:mm}");
                for (int i = 0; i < transaction.Items.Length; i++)
                {
                    Console.WriteLine($"   {transaction.Items[i],-20}  {transaction.Quantities[i],-3}   ${transaction.Prices[i],-6:F2}");
                }
            }
            InputHelper.Pause();
        }
        public async Task AddMoneyAsync()
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
    }
}
