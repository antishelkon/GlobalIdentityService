namespace GlobalIdentityService.Exceptions
{
    public class AlreadyExistsException : Exception
    {   
        public string Resource { get; }
        public IReadOnlyList<(string Name, string Value)> Fields { get; }

        public AlreadyExistsException(string resource, List<(string Name, string Value)> fields, Exception? innerException = null) 
            : base($"{resource} with {string.Join(", ", fields.Select(f => $"{f.Name}: {f.Value}"))} already exists.", innerException)
        {
            Resource = resource;
            Fields = Array.AsReadOnly(fields.ToArray());
        }
    }
}
