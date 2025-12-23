namespace Vending_Machine_System.Helpers
{
    public static class InputHelper
    {
        public static string Prompt(string message)
        {
            while (true)
            {
                Console.Write(message);
                var input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("Input cannot be empty.");
            }
        }

        public static string PromptPassword(string message)
        {
            while (true)
            {
                Console.Write(message);
                var password = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(password))
                    return password;
                Console.WriteLine("Password cannot be empty.");
            }
        }

        public static int PromptInt(string message, int min, int max)
        {
            while (true)
            {
                Console.Write($"{message} [{min}-{max}]: ");
                if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        public static float PromptPositiveFloat(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (float.TryParse(Console.ReadLine(), out var value) && value >= 0)
                    return value;
                Console.WriteLine("Please enter a valid positive number.");
            }
        }

        public static int PromptPositiveInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out var value) && value >= 0)
                    return value;
                Console.WriteLine("Please enter a valid positive integer.");
            }
        }

        public static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        public static bool Confirm(string message)
        {
            Console.Write($"{message} (Y/N): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }
    }
}