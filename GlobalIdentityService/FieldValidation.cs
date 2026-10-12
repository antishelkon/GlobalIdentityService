using GlobalIdentityService.Dto;
using GlobalIdentityService.Exceptions;
using System.Runtime.CompilerServices;

namespace GlobalIdentityService
{
    public static class FieldValidation
    {
        public static void ValidateNotBlank(this string? value, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(fieldName ?? "value", $"{fieldName} is requred");
        }
        public static void ValidateNoWhiteSpaces(this string? value, [CallerArgumentExpression(nameof(value))] string? fieldName = null)
        {
            if (!string.IsNullOrEmpty(value) && value.Any(char.IsWhiteSpace))
                throw new ValidationException(fieldName ?? "value", $"{fieldName} must not contain spaces");
        }

        public static void ValidateEmail(string? value, string fieldName)
        {
            ValidateNotBlank(value, fieldName);
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
                throw new ValidationException(fieldName ?? "value", $"{fieldName} length must be between {min} and {max}");
            }
        }
    }
}
