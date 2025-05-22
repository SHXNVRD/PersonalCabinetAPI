using Domain.Shared.Errors;
using Domain.Shared.Errors.Base;
using Microsoft.AspNetCore.Identity;

namespace Application.Extensions;

public static class IdentityErrorExtensions
{
    public static DomainError ToDomainError(this IdentityError error)
    {
        var code = error.Code;
        var description = error.Description;

        var validationErrorCodes = new[]
        {
            IdentityErrorCodes.PasswordRequiresDigit,
            IdentityErrorCodes.PasswordRequiresLower,
            IdentityErrorCodes.PasswordRequiresUpper,
            IdentityErrorCodes.PasswordRequiresNonAlphanumeric,
            IdentityErrorCodes.PasswordTooShort,
            IdentityErrorCodes.InvalidEmail,
            IdentityErrorCodes.InvalidToken,
            IdentityErrorCodes.InvalidRoleName,
            IdentityErrorCodes.InvalidUserName,
            
        };

        var duplicationErrorCodes = new[]
        {
            IdentityErrorCodes.DuplicateUserName,
            IdentityErrorCodes.DuplicateEmail,
            IdentityErrorCodes.DuplicateRoleName,
            IdentityErrorCodes.LoginAlreadyAssociated
        };

        var alreadyHasValueErrorCodes = new[]
        {
            IdentityErrorCodes.UserAlreadyHasPassword,
            IdentityErrorCodes.UserAlreadyInRole
        };

        var undefinedErrorCodes = new[]
        {
            IdentityErrorCodes.DefaultError,
            IdentityErrorCodes.ConcurrencyFailure,
            IdentityErrorCodes.UserLockoutNotEnabled
        };

        return code switch
        {
            _ when validationErrorCodes.Contains(code) => Errors.InvalidData.ValidationFailed(description),
            _ when duplicationErrorCodes.Contains(code) => Errors.Conflict.Duplicate(description),
            IdentityErrorCodes.PasswordMismatch => Errors.Conflict.Mismatch(description),
            IdentityErrorCodes.UserNotInRole => Errors.Forbidden.UserNotInRole(description),
            _ when alreadyHasValueErrorCodes.Contains(code) => Errors.Conflict.AlreadyHasValue(description),
            _ when undefinedErrorCodes.Contains(code) => Errors.Undefined.UndefinedError(description),
            _ => throw new InvalidCastException($"Failed to cast IdentityError to DomainError. Parameter {nameof(code)} out of range")
        };
    }
    
    private static class IdentityErrorCodes
    {
        public const string DefaultError = "DefaultError";
        public const string ConcurrencyFailure = "ConcurrencyFailure";
        public const string PasswordMismatch = "PasswordMismatch";
        public const string InvalidToken = "InvalidToken";
        public const string LoginAlreadyAssociated = "LoginAlreadyAssociated";
        public const string InvalidUserName = "InvalidUserName";
        public const string InvalidEmail = "InvalidEmail";
        public const string DuplicateUserName = "DuplicateUserName";
        public const string DuplicateEmail = "DuplicateEmail";
        public const string InvalidRoleName = "InvalidRoleName";
        public const string DuplicateRoleName = "DuplicateRoleName";
        public const string UserAlreadyHasPassword = "UserAlreadyHasPassword";
        public const string UserLockoutNotEnabled = "UserLockoutNotEnabled";
        public const string UserAlreadyInRole = "UserAlreadyInRole";
        public const string UserNotInRole = "UserNotInRole";
        public const string PasswordTooShort = "PasswordTooShort";
        public const string PasswordRequiresNonAlphanumeric = "PasswordRequiresNonAlphanumeric";
        public const string PasswordRequiresDigit = "PasswordRequiresDigit";
        public const string PasswordRequiresLower = "PasswordRequiresLower";
        public const string PasswordRequiresUpper = "PasswordRequiresUpper";
    }
}