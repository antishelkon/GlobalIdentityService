namespace GlobalIdentityService.Exceptions
{
    public class NotFoundException : Exception
    {
        public string FieldName { get; }
        public string FieldValue {  get; }
        public string Resource {  get; }
        public NotFoundException(string resuouce, string fieldName, string fieldValue, Exception? innerException = null)
            : base($"{resuouce} with {fieldName}: {fieldValue} not found", innerException)
        {
            Resource = resuouce;
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
    }
}
