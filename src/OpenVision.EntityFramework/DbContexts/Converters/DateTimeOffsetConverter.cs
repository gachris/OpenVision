using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OpenVision.EntityFramework.DbContexts.Converters;

/// <summary>
/// Converts between <see cref="DateTimeOffset"/> and <see cref="DateTime"/>.
/// </summary>
internal class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetConverter"/> class.
    /// </summary>
    public DateTimeOffsetConverter() : base(
        // Converts DateTimeOffset to DateTime (UTC DateTime).
        v => v.UtcDateTime,
        // Converts DateTime to DateTimeOffset with zero offset.
        v => new DateTimeOffset(v, TimeSpan.Zero))
    {
    }
}