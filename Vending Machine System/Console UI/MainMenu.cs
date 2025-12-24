using Backend.ApplicationConstants;
using Backend.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;

namespace Vending_Machine_System.Menus
{
    public class MainMenu
    {
        private readonly IUserService _userService;
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IAdminService _adminService;

        public MainMenu(IUserService userService, IItemService itemService, ITransactionService transactionService, IAdminService adminService)
        {
            _userService = userService;
            _itemService = itemService;
            _transactionService = transactionService;
            _adminService = adminService;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("WELCOME FROM VENDING MACHINE");

            while (true)
            {
                ShowMenu();
                var choice = InputHelper.PromptInt("Choice", 1, (int)MainMenuOption.Exit);

                switch ((MainMenuOption)choice)
                {
                    case MainMenuOption.AdminLogin:
                        await HandleAdminLoginAsync();
                        break;
                    case MainMenuOption.UserLogin:
                        await HandleUserLoginAsync();
                        break;
                    case MainMenuOption.Exit:
                        Console.WriteLine("Thank you for using Vending Machine!");
                        return;
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\n1. Admin Mode");
            Console.WriteLine("2. User Mode");
            Console.WriteLine("3. Exit");
        }

        private async Task HandleAdminLoginAsync()
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
                    var adminMenu = new AdminMenu(_itemService, _transactionService,_adminService);
                    await adminMenu.RunAsync();
                }
                else
                {
                    Console.WriteLine("Invalid username or password!");
                    InputHelper.Pause();
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private async Task HandleUserLoginAsync()
        {
            Console.Clear();
            Console.WriteLine("USER LOGIN / SIGNUP");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sign Up");
            Console.WriteLine("3. Back");

            var choice = InputHelper.PromptInt("Choice", 1, 3);
            switch (choice)
            {
                case 1: await LoginAsync(); break;
                case 2: await SignUpAsync(); break;
                case 3: break;
            }
        }

        private async Task LoginAsync()
        {
            var userName = InputHelper.Prompt("Username: ");
            var password = InputHelper.PromptPassword("Password: ");

            try
            {
                var user = await _userService.ValidateUserAsync(userName, password);
                if (user != null)
                {
                    Console.WriteLine($"Welcome back, {user.UserName}!");
                    InputHelper.Pause();
                    var userMenu = new UserMenu(user, _itemService, _transactionService, _userService);
                    await userMenu.RunAsync();
                }
                else
                {
                    Console.WriteLine("Invalid username or password!");
                    InputHelper.Pause();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                InputHelper.Pause();
            }
        }

        private async Task SignUpAsync()
        {
            var userName = InputHelper.Prompt($"Enter username (no comma ','; length {UserValidationRules.MinUserNameLength}–{UserValidationRules.MaxUserNameLength}): ");

            var password = InputHelper.PromptPassword($"Enter password (no comma ',', no spaces; length {UserValidationRules.MinPasswordLength}–{UserValidationRules.MaxPasswordLength}): ");


            try
            {
                await _userService.CreateUserAsync(userName, password);
                Console.WriteLine("Sign up successful! Please login to continue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign up failed: {ex.Message}");
            }
            InputHelper.Pause();
        }
    }
}
