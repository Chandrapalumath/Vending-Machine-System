using Backend.Services;
using DataLayer.Repositories.FileSystem;
using Vending_Machine_System.Menus;

namespace UserInterface
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var userRepo = new FileUserRepository();
            var itemRepo = new FileItemRepository();
            var transactionRepo = new FileTransactionRepository();

            // Change why to pass that much of arguments
            var userService = new UserService(userRepo, userRepo, userRepo);
            var itemService = new ItemService(itemRepo, itemRepo, itemRepo);
            var transactionService = new TransactionService(transactionRepo, userRepo, itemRepo, transactionRepo);

            var mainMenu = new MainMenu(userService, itemService, transactionService);
            await mainMenu.RunAsync();
        }
    }
}
