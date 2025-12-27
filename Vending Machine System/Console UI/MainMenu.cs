using Backend.ApplicationConstants;
using Backend.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;
using Vending_Machine_System.ApplicationConstants;
using Vending_Machine_System.Console_UI;

namespace Vending_Machine_System.Menus
{
    public class MainMenu
    {
        private AdminAuth _adminAuth;
        private UserAuth _userAuth;
        internal MainMenu(AdminAuth adminAuth, UserAuth userAuth)
        {
            _adminAuth = adminAuth;
            _userAuth = userAuth;
        }
        public async Task StartAsync()
        {
            Console.WriteLine("WELCOME FROM VENDING MACHINE");

            while (true)
            {
                ShowMenu();
                var choice = InputHelper.PromptInt("Choice", Constant.MainMenuMin, Constant.MainMenuMax);

                switch ((MainMenuOption)choice)
                {
                    case MainMenuOption.AdminLogin:
                        await _adminAuth.HandleAdminLoginAsync();
                        break;
                    case MainMenuOption.UserLogin:
                        await _userAuth.HandleUserLoginAsync();
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
    }
}
