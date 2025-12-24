namespace Backend.Exceptions
{
    internal class UserConflictException : Exception
    {
        public UserConflictException(string message) : base(message) { }
    }
}
