using DotnetCqrs.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);
var subFixInjections = new[] { "Service", "Repository", "Handler" };
builder.Services.AppInjection(subFixInjections);
var app = builder.Build();
app.AppMiddleWare();
app.MapControllers();
app.MapGet("/", () => new
{
    Timezones = TimeZoneInfo.Local,
    DateTime.Now,
    DateTime.UtcNow,
});
app.Run();