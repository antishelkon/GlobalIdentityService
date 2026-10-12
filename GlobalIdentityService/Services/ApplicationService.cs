using GlobalIdentityService.Data;
using GlobalIdentityService.Dto.Application;
using GlobalIdentityService.Exceptions;
using GlobalIdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalIdentityService.Services
{
    public class ApplicationService(IdentityDbContext _identityDbContext)
    {
        private const string ResourceName = "Application";

        private async Task<Application> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            Application? application = await _identityDbContext.Applications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (application == null)
                throw new NotFoundException(ResourceName, "id", id.ToString());
            return application;
        }

        public async Task<long> CreateAsync(CreateApplicationRequest applicationRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(applicationRequest);

            applicationRequest.Name.ValidateNotBlank();
            applicationRequest.Code.ValidateNotBlank();
            if (applicationRequest.Code.ToUpperInvariant() != applicationRequest.Code)
                throw new ValidationException("code", "Code must be in upper case");
            applicationRequest.Code.ValidateNoWhiteSpaces();
            /*if (await _identityDbContext.Applications.AnyAsync(a => a.Code == applicationRequest.Code, cancellationToken))
                throw new AlreadyExistsException(ResourceName, [("code", applicationRequest.Code)]);*/

            Application application = new Application
            {
                Name = applicationRequest.Name.Trim(),
                Code = applicationRequest.Code,
                Description = applicationRequest.Description,
                Enabled = true
            };

            _identityDbContext.Applications.Add(application);

            try
            {
                await _identityDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (DatabaseErrors.IsUniqueViolation(ex, "uq_application_code"))
            {
                throw new AlreadyExistsException(ResourceName, [("code", applicationRequest.Code)], ex);
            }

            return application.Id;
        }

        public async Task PatchAsync(long id, PatchApplicationRequest applicationRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(applicationRequest);

            if (applicationRequest.Name is null && applicationRequest.Description is null)
                throw new ValidationException("request", "At least one field must be provided");

            if (applicationRequest.Name is not null && string.IsNullOrWhiteSpace(applicationRequest.Name))
                throw new ValidationException("name", "Name must not be empty");

            var application = await GetByIdAsync(id, cancellationToken);

            application.Name = applicationRequest.Name?.Trim() ?? application.Name;
            application.Description = applicationRequest.Description ?? application.Description;

            await _identityDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(long id, UpdateApplicationRequest applicationRequest, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(applicationRequest);

            if (string.IsNullOrWhiteSpace(applicationRequest.Name))
                throw new ValidationException("name", "Name is required");

            var application = await GetByIdAsync(id, cancellationToken);

            application.Name = applicationRequest.Name.Trim();
            application.Description = applicationRequest.Description;

            await _identityDbContext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task SetEnabledAsync(long id, bool isEnabled, CancellationToken cancellationToken)
        {
            var application = await GetByIdAsync(id, cancellationToken);

            if (application.Enabled == isEnabled)
                return;
            application.Enabled = isEnabled;

            await _identityDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
