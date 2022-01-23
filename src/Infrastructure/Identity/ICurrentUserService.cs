namespace CleanArchitecture.Infrastructure.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
}
