namespace GlobalIdentityService.Exceptions
{
    public class ValidationException : Exception
    {
        public string FieldName { get; }

        public ValidationException(string fieldName, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            FieldName = fieldName;
        }
    }
}
