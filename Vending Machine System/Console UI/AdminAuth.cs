using Backend.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Menus;

namespace Vending_Machine_System.Console_UI
{
    internal class AdminAuth
    {
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IAdminService _adminService;

        internal AdminAuth(IItemService itemService, ITransactionService transactionService, IAdminService adminService)
        {
            _itemService = itemService;
            _transactionService = transactionService;
            _adminService = adminService;
        }
        internal async Task HandleAdminLoginAsync()
        {
            Console.Clear();
            Console.Write("Admin username: ");
            var username = Console.ReadLine()?.Trim();
            Console.Write("Admin password: ");
            var password = InputHelper.PromptPassword("");

            try
            {
                var admin = await _adminService.ValidateAdminAsync(username, password);
                if (admin != null)
                {
                    Console.WriteLine($"Welcome, {admin.UserName}!");
                    var adminMenu = new AdminMenu(_itemService, _transactionService, _adminService);
                    await adminMenu.RunAsync();
                }
                else
                {
                    Console.WriteLine("Invalid username or password!");
                    InputHelper.Pause();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
