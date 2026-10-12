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
        private const string ResourceName = "User";
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

        private async Task<User> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            User? user = await _identityDbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user is null)
                throw new NotFoundException(ResourceName, "id", id.ToString());
            return user;
        }

        public async Task<long> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(createUserRequest);
            _logger.LogDebug("Starting user creation for {username}", createUserRequest.Username);

            createUserRequest.Username.ValidateNotNullOrEmpty();
            createUserRequest.Email.ValidateNotNullOrEmpty();
            createUserRequest.Password.ValidateNotNullOrEmpty();

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
            user.Bio = "";
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
                throw new AlreadyExistsException(ResourceName, [("username", createUserRequest.Username)], ex);
            }
            catch (DbUpdateException ex) when (DatabaseErrors.IsUniqueViolation(ex, "uq_users_email") ||
                DatabaseErrors.IsUniqueViolation(ex, "ux_users_email_lower"))
            {
                throw new AlreadyExistsException(ResourceName, [("email", createUserRequest.Email)], ex);
            }

            _logger.LogInformation("User {userId} created", user.Id);
            return user.Id;
        }

        public async Task PatchUserAsync(long id, PatchUserRequest patchUserRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(patchUserRequest);
            _logger.LogDebug("Starting user patching for user {id}", id);

            if(patchUserRequest.Username is null && patchUserRequest.Bio is null && patchUserRequest.DisplayName is null)
                throw new ValidationException("request", "At least one field must be provided");

            patchUserRequest.Username.ValidateNotEmpty();

            patchUserRequest = patchUserRequest with
            {
                Username = patchUserRequest.Username?.Trim(),
                Bio = patchUserRequest.Bio?.Trim(),
                DisplayName = patchUserRequest.DisplayName?.Trim()
            };

            patchUserRequest.Username.ValidateLengthIfNotNull(4, 64);

            if (!string.IsNullOrEmpty(patchUserRequest.Bio))
                patchUserRequest.Bio.ValidateLength(0, 255);
            if (!string.IsNullOrEmpty(patchUserRequest.DisplayName))
                patchUserRequest.DisplayName.ValidateLength(0, 64);

            if (patchUserRequest.Username is not null)
                FieldValidation.ValidateNoWhiteSpaces(patchUserRequest.Username, "username");

            User? user = await GetByIdAsync(id, cancellationToken);

            user.Username = patchUserRequest.Username ?? user.Username;
            if(patchUserRequest.DisplayName is not null)
                user.DisplayName = string.IsNullOrEmpty(patchUserRequest.DisplayName) ? null : patchUserRequest.DisplayName;
            if (patchUserRequest.Bio is not null)
                user.Bio = string.IsNullOrEmpty(patchUserRequest.Bio) ? null : patchUserRequest.Bio;

            user.UpdatedAt = DateTime.UtcNow;

            //_userSecurityAuditService.AddRecord()

            try
            {
                await _identityDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (DatabaseErrors.IsUniqueViolation(ex, "uq_users_username") ||
                DatabaseErrors.IsUniqueViolation(ex, "ux_users_username_lower"))
            {
                throw new AlreadyExistsException(ResourceName, [("username", user.Username)], ex);
            }
        }
    }
}
