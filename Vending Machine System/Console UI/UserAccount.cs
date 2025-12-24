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
