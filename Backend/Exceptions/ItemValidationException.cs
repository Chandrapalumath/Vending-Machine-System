namespace Backend.Exceptions
{
    public class ItemValidationException : Exception
    {
        public ItemValidationException(string message) : base(message) { }
    }
}
