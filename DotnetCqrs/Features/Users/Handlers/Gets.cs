using Microsoft.EntityFrameworkCore;

namespace DotnetCqrs.Features.Users.Handlers;

public interface IGetAllUsersHandler
{
    Task<IEnumerable<UsersEntity>> HandleAsync(GetAllUsersQuery query);
}

public class GetAllUsersHandler(IUsersRepository repo) : IGetAllUsersHandler
{
    public async Task<IEnumerable<UsersEntity>> HandleAsync(GetAllUsersQuery query)
    {
        var users = await repo.GetAll()
            .ToListAsync();
        return users;
    }
}