using Backend.Interfaces;
using Backend.Model;
using Vending_Machine_System.Helpers;

namespace Vending_Machine_System.Menus
{
    public class UserShopping
    {
        private readonly IItemService _itemService;
        private readonly User _currentUser;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly UserAccount _accountService;
        public UserShopping(IItemService itemService, ITransactionService transactionService, IUserService userService, User user)
        {
            _currentUser = user;
            _itemService = itemService;
            _transactionService = transactionService;
            _userService = userService;
            _accountService = new UserAccount(_transactionService, _userService, _currentUser);
        }
        public async Task BuyItemsAsync()
        {
            var chosenItems = new List<string>();
            var chosenPrices = new List<float>();
            var chosenQty = new List<int>();

            var cartQuantities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                var items = await _itemService.GetAllItemsAsync();

                Console.Clear();
                Console.WriteLine("Available items:");
                Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-8}", "Idx", "Item", "Price", "Qty");

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var alreadySelected = cartQuantities.GetValueOrDefault(item.Name, 0);
                    var available = item.Quantity - alreadySelected;

                    if (available > 0)
                    {
                        Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-8}",
                            i, item.Name, item.Price, available);
                    }
                }

                Console.Write("\nEnter item index to buy or 'exit': ");
                var input = Console.ReadLine()?.Trim().ToLower();

                if (input == "exit")
                    break;

                if (!int.TryParse(input, out var index) || index < 0 || index >= items.Count)
                {
                    Console.WriteLine("Invalid index.");
                    InputHelper.Pause();
                    continue;
                }

                var selectedItem = items[index];
                var alreadyInCart = cartQuantities.GetValueOrDefault(selectedItem.Name, 0);
                var availableQty = selectedItem.Quantity - alreadyInCart;

                if (availableQty <= 0)
                {
                    Console.WriteLine("Item already fully selected.");
                    InputHelper.Pause();
                    continue;
                }

                var quantity = InputHelper.PromptPositiveInt(
                    $"Enter quantity for {selectedItem.Name} (Available: {availableQty}): ");

                if (quantity > availableQty)
                {
                    Console.WriteLine("Not enough stock available.");
                    InputHelper.Pause();
                    continue;
                }
                cartQuantities[selectedItem.Name] = alreadyInCart + quantity;

                chosenItems.Add(selectedItem.Name);
                chosenPrices.Add(selectedItem.Price);
                chosenQty.Add(quantity);

                Console.WriteLine($"Added {quantity} x {selectedItem.Name}");
                InputHelper.Pause();
            }

            if (chosenItems.Count == 0)
            {
                Console.WriteLine("No items selected.");
                InputHelper.Pause();
                return;
            }
            float totalAmount = 0;
            for (int i = 0; i < chosenItems.Count; i++)
                totalAmount += chosenPrices[i] * chosenQty[i];

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Total Amount: ${totalAmount:F2}");
                Console.WriteLine($"Wallet Balance: ${_currentUser.Wallet:F2}");

                if (totalAmount > _currentUser.Wallet)
                {
                    Console.WriteLine("Insufficient wallet balance.");
                    if (InputHelper.Confirm("Add money to wallet?"))
                    {
                        await _accountService.AddMoneyAsync();
                        continue;
                    }

                    Console.WriteLine("Order cancelled.");
                    InputHelper.Pause();
                    return;
                }

                var finalItems = await _itemService.GetAllItemsAsync();
                bool conflict = false;

                foreach (var kvp in cartQuantities)
                {
                    var item = finalItems.FirstOrDefault(x =>
                        x.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));

                    if (item == null || item.Quantity < kvp.Value)
                    {
                        conflict = true;
                        break;
                    }
                }

                if (conflict)
                {
                    Console.WriteLine("Item quantity changed. Please try again.");
                    InputHelper.Pause();
                    return;
                }

                foreach (var items in cartQuantities)
                {
                    var item = finalItems.First(x =>
                        x.Name.Equals(items.Key, StringComparison.OrdinalIgnoreCase));

                    item.Quantity -= items.Value;
                }

                foreach (var item in finalItems)
                {
                    await _itemService.UpdateItemAsync(item.Name, null, null, item.Quantity);
                }

                _currentUser.Wallet -= totalAmount;
                await _userService.UpdateUserWalletAsync(
                    _currentUser.UserName, _currentUser.Wallet);

                var transaction = new Transaction(
                    _currentUser.UserName,
                    chosenItems.ToArray(),
                    chosenPrices.ToArray(),
                    chosenQty.ToArray(),
                    totalAmount);

                await _transactionService.AddTransactionAsync(transaction);

                Console.WriteLine("Purchase successful! Items dispensed.");
                InputHelper.Pause();
                return;
            }
        }

    }
}
