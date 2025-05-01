using DotnetCqrs.Infrastructure.Data;

namespace DotnetCqrs.Features.Users;

public interface IUsersRepository : IRepository<UsersEntity>;

public class UsersRepository(AppDbContext context) : Repository<UsersEntity>(context), IUsersRepository;