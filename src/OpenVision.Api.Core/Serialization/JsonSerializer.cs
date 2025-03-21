using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenVision.Api.Core.Serialization;

/// <summary>
/// A serializer implementation that handles object serialization and deserialization to/from JSON format.
/// </summary>
public class JsonSerializer : ISerializer
{
    private static JsonSerializer _instance;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Gets the singleton instance of the <see cref="JsonSerializer"/>.
    /// </summary>
    public static JsonSerializer Instance => _instance ??= new JsonSerializer();

    /// <summary>
    /// Gets the format supported by this serializer, which is "json" for JSON format.
    /// </summary>
    public string Format => "json";

    /// <summary>
    /// Construct a new json serializer.
    /// </summary>
    public JsonSerializer()
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        _jsonSerializerOptions.Converters.Add(new RFC3339DateTimeConverter());
    }

    /// <summary>
    /// Serializes the specified object into a Stream in JSON format.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="target">The target stream into which to serialize the object.</param>
    public void Serialize(object obj, Stream target)
    {
        using var writer = new Utf8JsonWriter(target);
        obj ??= string.Empty;
        System.Text.Json.JsonSerializer.Serialize(writer, obj, _jsonSerializerOptions);
    }

    /// <summary>
    /// Serializes the specified object into an JSON string.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <returns>A string representation of the serialized object in JSON format.</returns>
    public string Serialize(object obj)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        obj ??= string.Empty;
        System.Text.Json.JsonSerializer.Serialize(writer, obj, _jsonSerializerOptions);
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Deserializes the JSON string into an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the JSON string into.</typeparam>
    /// <param name="input">The JSON string to be deserialized.</param>
    /// <returns>The deserialized object of type T.</returns>
    public T Deserialize<T>(string input) => string.IsNullOrEmpty(input) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(input, _jsonSerializerOptions);

    /// <summary>
    /// Deserializes the JSON string into an object of the specified type.
    /// </summary>
    /// <param name="input">The JSON string to be deserialized.</param>
    /// <param name="type">The type of object to deserialize the JSON string into.</param>
    /// <returns>The deserialized object.</returns>
    public object Deserialize(string input, Type type) => string.IsNullOrEmpty(input) ? null : System.Text.Json.JsonSerializer.Deserialize(input, type, _jsonSerializerOptions);

    /// <summary>
    /// Deserializes the JSON stream into an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the JSON stream into.</typeparam>
    /// <param name="stream">The JSON stream to be deserialized.</param>
    /// <returns>The deserialized object of type T.</returns>
    public T Deserialize<T>(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var document = JsonDocument.Parse(streamReader.ReadToEnd());
        return System.Text.Json.JsonSerializer.Deserialize<T>(document.RootElement.GetRawText(), _jsonSerializerOptions);
    }
}
