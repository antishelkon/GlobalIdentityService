namespace GlobalIdentityService.Dto
{
    public class UserRequests
    {
    }
    public record CreateUserRequest(
        string Username,
        string Email,
        string Password,
        string? DisplayName = null
        );

}
