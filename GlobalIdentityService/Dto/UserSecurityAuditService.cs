using GlobalIdentityService.Data;

namespace GlobalIdentityService.Dto
{
    public class UserSecurityAuditService
    {
        private readonly IdentityDbContext _identityDbContext;

        public UserSecurityAuditService(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public enum EventType { Created, Changed}

        public Task AddRecord(long targetId, EventType eventType)
        {
            throw new NotImplementedException();
        }
    }
}
