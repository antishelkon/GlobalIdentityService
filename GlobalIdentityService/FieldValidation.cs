using GlobalIdentityService.Dto;
using GlobalIdentityService.Exceptions;
using System.Runtime.CompilerServices;

namespace GlobalIdentityService
{
    public static class FieldValidation
    {
        public static void ValidateNotNullOrEmpty(this string? value, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var name = fieldName ?? "value";
                throw new ValidationException(name, $"{name} is requred");
            }
        }
        public static void ValidateNoWhiteSpaces(this string? value, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (!string.IsNullOrEmpty(value) && value.Any(char.IsWhiteSpace))
            {
                var name = fieldName ?? "value";
                throw new ValidationException(name, $"{name} must not contain spaces");
            }
        }

        public static void ValidateEmail(string? value, string fieldName)
        {
            ValidateNotNullOrEmpty(value, fieldName);
            ValidateNoWhiteSpaces(value, fieldName);

            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value))
            {
                throw new ValidationException(fieldName, "Invalid email format");
            }
        }
        public static void ValidateLength(this string value, int min, int max, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (value.Length < min || value.Length > max)
            {
                var name = fieldName ?? "value";
                throw new ValidationException(name, $"{name} length must be between {min} and {max}");
            }
        }
        public static void ValidateLengthIfNotNull(this string? value, int min, int max, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (value is null)
                return;
            ValidateLength(value, min, max, fieldName);
        }
        public static void ValidateNotEmpty(this string? value, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
            {
                var name = fieldName ?? "value";
                throw new ValidationException(name, $"{name} must not be null");
            }
        }
    }
}
