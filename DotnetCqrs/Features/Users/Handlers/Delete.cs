using DotnetCqrs.Infrastructure.Configuration;

namespace DotnetCqrs.Features.Users.Handlers;

public interface IDeleteUserHandler
{
    Task HandleAsync(int id);
}

public class DeleteUserHandler(IUsersRepository repo) : IDeleteUserHandler
{
    public async Task HandleAsync(int id)
    {
        var user = await repo.GetSingleAsync(e => e.Id == id);
        if (user == null) throw new NotFoundEx();
        repo.Remove(user);
        await repo.CommitAsync();
    }
}