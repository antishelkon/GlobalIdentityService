using GlobalIdentityService.Data;
using GlobalIdentityService.Dto;
using GlobalIdentityService.Exceptions;
using GlobalIdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlobalIdentityService.Services
{
    public class UserService
    {
        private readonly IdentityDbContext _identityDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly UserSecurityAuditService _userSecurityAuditService;

        public UserService(IdentityDbContext identityDbContext, IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger, UserSecurityAuditService userSecurityAuditService)
        {
            _identityDbContext = identityDbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _userSecurityAuditService = userSecurityAuditService;
        }

        public async Task<long> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(createUserRequest);
            _logger.LogDebug("Starting user creation for {username}", createUserRequest.Username);

            createUserRequest.Username.ValidateNotBlank();
            createUserRequest.Email.ValidateNotBlank();
            createUserRequest.Password.ValidateNotBlank();

            createUserRequest = createUserRequest with
            {
                Email = createUserRequest.Email.Trim(),
                Username = createUserRequest.Username.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(createUserRequest.DisplayName) ? null : createUserRequest.DisplayName.Trim()
            };

            createUserRequest.Username.ValidateLength(4, 64);
            createUserRequest.Email.ValidateLength(4, 254);
            createUserRequest.Password.ValidateLength(8, 64);
            if (!string.IsNullOrEmpty(createUserRequest.DisplayName))
                createUserRequest.DisplayName.ValidateLength(1, 64);

            createUserRequest.Username.ValidateNoWhiteSpaces();
            FieldValidation.ValidateEmail(createUserRequest.Email, "Email");

            User user = new User
            {
                Username = createUserRequest.Username,
                Email = createUserRequest.Email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, createUserRequest.Password);
            user.DisplayName = createUserRequest.DisplayName;
            var now = DateTime.UtcNow;
            user.CreatedAt = now;
            user.UpdatedAt = now;
            user.EmailVerified = false;
            user.Status = "Active";

            _identityDbContext.Users.Add(user);

            //_userSecurityAuditService.AddRecord();

            try
            {
                await _identityDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (DatabaseErrors.IsUniqueViolation(ex, "uq_users_username") ||
                DatabaseErrors.IsUniqueViolation(ex, "ux_users_username_lower"))
            {
                throw new AlreadyExistsException("User", [("username", createUserRequest.Username)], ex);
            }
            catch (DbUpdateException ex) when (DatabaseErrors.IsUniqueViolation(ex, "uq_users_email") ||
                DatabaseErrors.IsUniqueViolation(ex, "ux_users_email_lower"))
            {
                throw new AlreadyExistsException("User", [("email", createUserRequest.Email)], ex);
            }

            _logger.LogInformation("User {userId} created", user.Id);
            return user.Id;
        }
    }
}
