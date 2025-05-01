namespace DotnetCqrs.Features.Users;

public record CreateUserCommand(string Username, string Email, DateTime? StartDate, TimeOnly? StartTime);

public record UpdateUserCommand(int Id, string Username, string Email, DateTime? StartDate, TimeOnly? StartTime);