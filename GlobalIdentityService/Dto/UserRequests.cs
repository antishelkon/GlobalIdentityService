namespace GlobalIdentityService.Dto
{
    public class UserRequests
    {
    }
    public record CreateUserRequest(
        string Username,
        string Email,
        string Password,
        string? DisplayName = null);

    public record PatchUserRequest(
        string? Username = null,
        string? Bio = null,
        string? DisplayName = null);
}
