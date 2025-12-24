using Backend.Exceptions;
using Backend.Interfaces;
using DataLayer.Repositories.Interfaces;
using Vending_Machine_System.Helpers;
using Vending_Machine_System.Models.Enums;

namespace Vending_Machine_System.Menus
{
    public class AdminMenu
    {
        private readonly IItemService _itemService;
        private readonly ITransactionService _transactionService;

        public AdminMenu(IItemService itemService, ITransactionService transactionService)
        {
            _itemService = itemService;
            _transactionService = transactionService;
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
            var name = InputHelper.Prompt("Enter item name (no ',' or '|'): ");
            var price = InputHelper.PromptPositiveFloat("Price: ");
            var quantity = InputHelper.PromptPositiveInt("Quantity: ");

            try
            {
                await _itemService.AddItemAsync(name, price, quantity);
                Console.WriteLine("Item added successfully!");
                InputHelper.Pause();
            }
            catch (ItemValidationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (NegativeValueException exception)
            {
                Console.WriteLine(exception.Message + " cannot be negative");
            }
            catch (ItemAlreadyExistsException exception)
            {
                Console.WriteLine(exception.Message + "Already Exists");
            }
        }

        private async Task RemoveItemAsync()
        {
            Console.Clear();
            var name = InputHelper.Prompt("Item name to remove : ");
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
            var transactions = await _transactionService.GetAllTransactionsAsync();

            if (!transactions.Any())
            {
                Console.WriteLine("No transactions found.");
                InputHelper.Pause();
                return;
            }

            foreach (var transaction in transactions)
            {
                Console.WriteLine($"{transaction.UserName,-15}  ${transaction.TotalAmount,-10:F2} {transaction.TimeUtc:yyyy-MM-dd HH:mm}");
                for (int i = 0; i < transaction.Items.Length; i++)
                {
                    Console.WriteLine($"    {transaction.Items[i],-20} x{transaction.Quantities[i],-3}  ${transaction.Prices[i],-6:F2}");
                }
            }
            InputHelper.Pause();
        }
    }
}
