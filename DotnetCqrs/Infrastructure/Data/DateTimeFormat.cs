using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotnetCqrs.Infrastructure.Data;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format = Environment.GetEnvironmentVariable("DATETIME_FORMAT") ?? "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.ParseExact(reader.GetString()!, _format, null);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_format));
}

public class CustomNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string _format = Environment.GetEnvironmentVariable("DATETIME_FORMAT") ?? "yyyy-MM-dd HH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => string.IsNullOrWhiteSpace(reader.GetString())
            ? null
            : DateTime.ParseExact(reader.GetString()!, _format, null);

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString(_format));
        else
            writer.WriteNullValue();
    }
}
