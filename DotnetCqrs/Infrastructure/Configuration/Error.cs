using Microsoft.AspNetCore.Diagnostics;


namespace DotnetCqrs.Infrastructure.Configuration;

internal static class ErrorConfiguration
{
    public static void AppError(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    if (contextFeature.Error is AppEx badRequestException)
                    {
                        context.Response.StatusCode = badRequestException.StatusCode;
                    }

                    await context.Response.WriteAsync(contextFeature.Error.Message);
                }
            });
        });
    }
}

public class AppEx(string message, int statusCode = StatusCodes.Status400BadRequest)
    : Exception(message)
{
    public int StatusCode => statusCode;
}

public class ExistedEx(string item = "item")
    : AppEx($"{item} already existed");

public class NotFoundEx(string item = "item")
    : AppEx($"{item} not found");

public class MessageEx(string message) : AppEx(message);

public class UnAuthorizedEx(string message) : AppEx(message, StatusCodes.Status401Unauthorized);

public class ForbidEx(string message = "You are no permission") : AppEx(message, StatusCodes.Status403Forbidden);