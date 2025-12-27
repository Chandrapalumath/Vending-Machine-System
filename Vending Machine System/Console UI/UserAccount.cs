using Backend.Interfaces;
using Backend.Model;
using Vending_Machine_System.Helpers;

namespace Vending_Machine_System.Menus
{
    public class UserAccount
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        public UserAccount(ITransactionService transactionService, IUserService userService)
        {
            _transactionService = transactionService;
            _userService = userService;
        }
        public async Task ShowUserTransactionsAsync(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSACTIONS ===");
            var transactions = await _transactionService.GetUserTransactionsAsync(currentUser.UserName);

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
        public async Task AddMoneyAsync(User currentUser)
        {
            var amount = InputHelper.PromptPositiveFloat("Amount to add: ");
            var newWallet = currentUser.Wallet + amount;

            try
            {
                await _userService.UpdateUserWalletAsync(currentUser.UserName, newWallet);
                currentUser.Wallet = newWallet;
                Console.WriteLine($"Added ${amount:F2}! New balance: ${currentUser.Wallet:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add money: {ex.Message}");
            }
            InputHelper.Pause();
        }
    }
}
