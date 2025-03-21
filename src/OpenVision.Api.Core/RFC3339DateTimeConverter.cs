using System.Text.Json;
using System.Text.Json.Serialization;
using OpenVision.Api.Core.Util;

namespace OpenVision.Api.Core;

/// <summary>
/// A custom JSON converter for handling DateTime objects using RFC 3339 format.
/// </summary>
public class RFC3339DateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Reads and converts the JSON to a DateTime object.
    /// This method is not implemented because reading is not necessary for this converter.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>A DateTime object.</returns>
    /// <exception cref="NotImplementedException">Thrown because reading is unnecessary.</exception>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Unnecessary because CanRead is false.");
    }

    /// <summary>
    /// Determines whether this converter can convert the specified object type.
    /// </summary>
    /// <param name="objectType">The type to check.</param>
    /// <returns>
    ///   <c>true</c> if this converter can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }

    /// <summary>
    /// Writes a DateTime object as a JSON string in RFC 3339 format.
    /// </summary>
    /// <param name="writer">The writer to which to write.</param>
    /// <param name="value">The DateTime value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Utilities.ConvertToRFC3339(value));
    }
}
