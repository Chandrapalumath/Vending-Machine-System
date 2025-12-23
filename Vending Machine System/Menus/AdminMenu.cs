using Backend.Exceptions;
using Backend.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;

namespace Vending_Machine_System.Menus
{
    public class AdminMenu
    {
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;

        public AdminMenu(
            IItemService itemService,
            ITransactionService transactionService,
            IUserService userService)
        {
            _itemService = itemService;
            _transactionService = transactionService;
            _userService = userService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                ShowMenu();
                var choice = InputHelper.PromptInt("Choice", 1, (int)AdminMenuOption.Exit);

                try
                {
                    switch ((AdminMenuOption)choice)
                    {
                        case AdminMenuOption.ShowItemList: await ShowItemListAsync(); break;
                        case AdminMenuOption.AddItem: await AddItemAsync(); break;
                        case AdminMenuOption.RemoveItem: await RemoveItemAsync(); break;
                        case AdminMenuOption.UpdateItem: await UpdateItemAsync(); break;
                        case AdminMenuOption.TransactionHistory: await ShowTransactionsAsync(); break;
                        case AdminMenuOption.Exit: return;
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
            Console.WriteLine("=== ADMIN PANEL ===");
            Console.WriteLine("1. Show Item List");
            Console.WriteLine("2. Add Item");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. Update Item");
            Console.WriteLine("5. Transaction History");
            Console.WriteLine("6. Exit");
        }

        private async Task ShowItemListAsync()
        {
            Console.Clear();
            var items = await _itemService.GetAllItemsAsync();
            Console.WriteLine("=== ITEM INVENTORY ===");
            Console.WriteLine("{0,-20} {1,-10} {2,-10}", "Item", "Price", "Quantity");

            foreach (var item in items)
            {
                Console.WriteLine("{0,-20} ${1,-9:F2} {2,-9}", item.Name, item.Price, item.Quantity);
            }
            InputHelper.Pause();
        }

        private async Task AddItemAsync()
        {
            Console.Clear();
            var name = InputHelper.Prompt("Item name (no ',' or '|'): ");
            var price = InputHelper.PromptPositiveFloat("Price: ");
            var quantity = InputHelper.PromptPositiveInt("Quantity: ");

            await _itemService.AddItemAsync(name, price, quantity);
            Console.WriteLine("Item added successfully!");
            InputHelper.Pause();
        }

        private async Task RemoveItemAsync()
        {
            Console.Clear();
            var name = InputHelper.Prompt("Item name to remove: ");
            await _itemService.RemoveItemAsync(name);
            Console.WriteLine("Item removed successfully!");
            InputHelper.Pause();
        }

        private async Task UpdateItemAsync()
        {
            Console.Clear();
            var name = InputHelper.Prompt("Item name to update: ");
            Console.WriteLine("\n1. Item Name");
            Console.WriteLine("2. Item Price");
            Console.WriteLine("3. Item Quantity");
            var choice = InputHelper.PromptInt("Choice", 1, 3);

            try
            {
                switch ((UpdateItemOption)choice)
                {
                    case UpdateItemOption.ItemName:
                        var newName = InputHelper.Prompt("New name: ");
                        await _itemService.UpdateItemAsync(name, newName, null, null);
                        break;
                    case UpdateItemOption.ItemPrice:
                        var newPrice = InputHelper.PromptPositiveFloat("New price: ");
                        await _itemService.UpdateItemAsync(name, null, newPrice, null);
                        break;
                    case UpdateItemOption.ItemQuantity:
                        var newQuantity = InputHelper.PromptPositiveInt("New quantity: ");
                        await _itemService.UpdateItemAsync(name, null, null, newQuantity);
                        break;
                }
                Console.WriteLine("Item updated successfully!");
            }
            catch (ItemNotFoundException)
            {
                Console.WriteLine("Item not found!");
            }
            catch (ItemValidationException ex)
            {
                Console.WriteLine($"Validation: {ex.Message}");
            }
            catch (NegativeValueException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            InputHelper.Pause();
        }

        private async Task ShowTransactionsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== RECENT TRANSACTIONS ===");
            var transactions = await _transactionService.GetAllTransactionsAsync(5);

            if (!transactions.Any())
            {
                Console.WriteLine("No transactions found.");
                InputHelper.Pause();
                return;
            }

            foreach (var txn in transactions)
            {
                Console.WriteLine($"{txn.UserName,-15}  ${txn.TotalAmount,-10:F2} {txn.TimeUtc:yyyy-MM-dd HH:mm}");
                for (int i = 0; i < txn.Items.Length; i++)
                {
                    Console.WriteLine($"    {txn.Items[i],-20} x{txn.Quantities[i],-3}  ${txn.Prices[i],-6:F2}");
                }
            }
            InputHelper.Pause();
        }
    }
}
