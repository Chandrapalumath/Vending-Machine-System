namespace Backend.Exceptions
{
    public class NegativeValueException : Exception
    {
        public NegativeValueException(string fieldName)
            : base($"{fieldName} cannot be negative.") { }
    }
}
