namespace DotnetCqrs.Features.Users;

public class UpdateUserRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public TimeOnly? StartTime { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public TimeOnly? StartTime { get; set; }
}