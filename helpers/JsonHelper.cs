using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fagkaffe.Helpers;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;
        return DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.FFFZ", CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddThh:mm:ss.FFFZ"));
    }
}