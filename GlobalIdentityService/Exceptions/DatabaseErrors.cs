using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GlobalIdentityService.Exceptions
{
    public static class DatabaseErrors
    {
        public static bool IsUniqueViolation(DbUpdateException exception, string constraintName)
        {
            return exception.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.UniqueViolation &&
                pg.ConstraintName == constraintName;
        }
    }
}
