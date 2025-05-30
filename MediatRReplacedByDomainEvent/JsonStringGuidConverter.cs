using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediatRReplacedByDomainEvent
{
    public class JsonStringGuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (Guid.TryParse(str, out var guid))
                {
                    return guid;
                }

                // 尝试处理不带连字符的 GUID
                if (str?.Length == 32)
                {
                    if (Guid.TryParseExact(str, "N", out guid))
                    {
                        return guid;
                    }
                }
            }

            throw new JsonException("Invalid GUID format");
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("D"));
        }
    }
}
