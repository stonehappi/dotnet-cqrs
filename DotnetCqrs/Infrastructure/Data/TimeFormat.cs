using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotnetCqrs.Infrastructure.Data;

public class CustomTimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _format = Environment.GetEnvironmentVariable("TIME_FORMAT") ?? "HH:mm:ss";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TimeOnly.ParseExact(reader.GetString()!, _format);

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_format));
}

public class CustomNullableTimeOnlyConverter : JsonConverter<TimeOnly?>
{
    private readonly string _format = Environment.GetEnvironmentVariable("TIME_FORMAT") ?? "HH:mm:ss";

    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => string.IsNullOrWhiteSpace(reader.GetString()) ? null : TimeOnly.ParseExact(reader.GetString()!, _format);

    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString(_format));
        else
            writer.WriteNullValue();
    }
}