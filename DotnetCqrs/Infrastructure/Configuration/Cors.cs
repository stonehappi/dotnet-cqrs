namespace DotnetCqrs.Infrastructure.Configuration;

internal static class CorsConfiguration
{
    public static void AppCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                    policy.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
            );
        });
    }
}