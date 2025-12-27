using Backend.ApplicationConstants;
using Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Menus;

namespace Vending_Machine_System.Console_UI
{
    internal class UserAuth
    {
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly UserAccount _userAccount;

        internal UserAuth(IItemService itemService, ITransactionService transactionService, IUserService userService, UserAccount userAccount)
        {
            _itemService = itemService;
            _transactionService = transactionService;
            _userService = userService;
            _userAccount = userAccount;
        }
        internal async Task HandleUserLoginAsync()
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

        internal async Task LoginAsync()
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
                    var userShopping = new UserShopping(_itemService, _transactionService, _userService,user, _userAccount);
                    var userMenu = new UserMenu(user, _itemService, _transactionService, _userService, userShopping, _userAccount);
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

        internal async Task SignUpAsync()
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
