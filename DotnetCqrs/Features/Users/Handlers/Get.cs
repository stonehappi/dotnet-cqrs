using DotnetCqrs.Infrastructure.Configuration;

namespace DotnetCqrs.Features.Users.Handlers;

public interface IGetUserByIdHandler
{
    Task<UsersEntity> HandleAsync(GetUserByIdQuery query);
}

public class GetUserByIdHandler(IUsersRepository repo) : IGetUserByIdHandler
{
    public async Task<UsersEntity> HandleAsync(GetUserByIdQuery query)
    {
        var user = await repo.GetSingleAsync(e => e.Id == query.Id);
        if (user == null) throw new NotFoundEx();
        return user;
    }
}