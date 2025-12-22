namespace Backend.Exceptions
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException(string itemName)
            : base($"Item '{itemName}' already exists.") { }
    }
}
