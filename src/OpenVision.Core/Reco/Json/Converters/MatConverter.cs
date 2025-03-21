using System.Text.Json;
using System.Text.Json.Serialization;
using OpenVision.Core.Utils;

namespace OpenVision.Core.Reco.Json.Converters;

internal class MatConverter : JsonConverter<Mat>
{
    public override Mat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Ensure the token is a start of an array
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected a JSON array to deserialize Mat object.");
        }

        // Read byte array from JSON
        var data = new List<byte>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType != JsonTokenType.Number || !reader.TryGetByte(out byte value))
            {
                throw new JsonException("Expected byte values in the JSON array.");
            }

            data.Add(value);
        }

        var byteArray = data.ToArray();

        return byteArray.ToMat();
    }

    public override void Write(Utf8JsonWriter writer, Mat value, JsonSerializerOptions options)
    {
        // Serialize Mat as a byte array
        writer.WriteStartArray();

        // Convert the Mat object to a byte array
        var byteArray = value.ToArray();

        // Write each byte to the JSON array
        foreach (var b in byteArray)
        {
            writer.WriteNumberValue(b);
        }

        writer.WriteEndArray();
    }
}