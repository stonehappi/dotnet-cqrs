using System.Reflection;
using DotnetCqrs.Infrastructure.Data;

namespace DotnetCqrs.Infrastructure.Configuration;

public static class InjectionConfiguration
{
    public static void AppMiddleWare(this IApplicationBuilder app)
    {
        var env = Environment.GetEnvironmentVariable("ENVIRONMENT");
        if (env != "Prod") app.AppSwagger();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.AppError();
    }

    public static void AppInjection(this IServiceCollection service, string[] subFix)
    {
        foreach (var sub in subFix)
        {
            _assemblyInjection(service, sub);
        }

        service.AppCors();
        service.AppSwagger();
        // service.AppAuthentication();
        service.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomNullableDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomTimeOnlyConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomNullableTimeOnlyConverter());
            });
        service.AppDatabase();
    }

    private static void _assemblyInjection(IServiceCollection service, string subFix)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(a => a.Name.EndsWith(subFix) && a is { IsAbstract: false, IsInterface: false })
            .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            .ToList()
            .ForEach(typesToRegister =>
            {
                if (subFix.Contains("Singleton"))
                {
                    typesToRegister.serviceTypes.ForEach(typeToRegister =>
                        service.AddSingleton(typeToRegister, typesToRegister.assignedType));
                }
                else
                {
                    typesToRegister.serviceTypes.ForEach(typeToRegister =>
                        service.AddScoped(typeToRegister, typesToRegister.assignedType));
                }
            });
    }
}