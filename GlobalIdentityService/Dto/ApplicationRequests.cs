namespace GlobalIdentityService.Dto.Application
{
    public record CreateApplicationRequest(
        string Name,
        string Code,
        string? Description = null);

    public record PatchApplicationRequest(
        string? Name = null,
        string? Description = null);

    public record UpdateApplicationRequest(
        string Name,
        string? Description = null);
}
