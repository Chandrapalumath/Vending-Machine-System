namespace Backend.Exceptions
{
    internal class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }
}
