using Backend.Interfaces;
using Backend.Services;
using DataLayer.Repositories.FileSystem;
using DataLayer.Repositories.Interfaces;
using Vending_Machine_System.Menus;

namespace UserInterface
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IUserRepository userRepo = new FileUserRepository();
            IItemRepository itemRepo = new FileItemRepository();
            ITransactionRepository transactionRepo = new FileTransactionRepository();
            IAdminRepository adminrepo = new FileAdminRepository();

            IUserService userService = new UserService(userRepo);
            IItemService itemService = new ItemService(itemRepo);
            IAdminService adminservice = new AdminService(adminrepo);
            ITransactionService transactionService = new TransactionService(transactionRepo);

            var mainMenu = new MainMenu(userService, itemService, transactionService, adminservice);
            await mainMenu.StartAsync();
        }
    }
}
