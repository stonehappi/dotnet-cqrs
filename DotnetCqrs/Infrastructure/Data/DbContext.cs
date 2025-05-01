using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DotnetCqrs.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var models = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());
        foreach (var relationship in models)
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

public static class DatabaseData
{
    private static readonly string DbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION") ??
                                                  "Server=localhost:5432;Database=cqrs;User Id=postgres;Password=password;";

    public static void AppDatabase(this IServiceCollection service)
    {
        service.AddDbContext<AppDbContext>(options => { options.UseNpgsql(DbConnection); });
    }
}