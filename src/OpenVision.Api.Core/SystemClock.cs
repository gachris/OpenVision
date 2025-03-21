namespace OpenVision.Api.Core;

/// <summary>
/// A default clock implementation that wraps the <see cref="DateTime.Now"/> and <see cref="DateTime.UtcNow"/> properties.
/// </summary>
public class SystemClock : IClock
{
    /// <summary>
    /// The default instance of the <see cref="SystemClock"/>.
    /// </summary>
    public static readonly IClock Default = new SystemClock();

    /// <summary>
    /// Constructs a new instance of <see cref="SystemClock"/>.
    /// </summary>
    protected SystemClock()
    {
    }

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}