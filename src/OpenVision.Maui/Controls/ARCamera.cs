using OpenVision.Core.Reco;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Represents an augmented reality camera view with tracking capabilities.
/// </summary>
public class ARCamera : View
{
    #region Fields/Consts

    /// <summary>
    /// Event raised when a target match is found during tracking.
    /// </summary>
    public event EventHandler<TargetMatchingEventArgs>? TrackFound;

    /// <summary>
    /// Event raised when tracking of a target is lost.
    /// </summary>
    public event EventHandler? TrackLost;

    /// <summary>
    /// Gets or sets a value indicating whether tracking is enabled.
    /// </summary>
    public static readonly BindableProperty IsTrackingEnabledProperty =
        BindableProperty.Create(nameof(IsTrackingEnabled), typeof(bool), typeof(ARCamera), true);

    /// <summary>
    /// Gets or sets a value of recognition service.
    /// </summary>
    private static readonly BindableProperty RecognitionProperty =
        BindableProperty.Create(nameof(Recognition), typeof(IRecognition), typeof(ARCamera), default(IRecognition));

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether tracking is enabled for the AR camera.
    /// </summary>
    public bool IsTrackingEnabled
    {
        get => (bool)GetValue(IsTrackingEnabledProperty);
        set => SetValue(IsTrackingEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value of recognition service.
    /// </summary>
    internal IRecognition Recognition
    {
        get => (IRecognition)GetValue(RecognitionProperty);
        private set => SetValue(RecognitionProperty, value);
    }

    #endregion

    /// <summary>
    /// Constructs an instance of the ARCamera class.
    /// </summary>
    public ARCamera()
    {
    }

    #region Methods

    /// <summary>
    /// Raises the TrackFound event with the provided event arguments.
    /// </summary>
    /// <param name="e">Event arguments containing information about the tracked target.</param>
    internal void OnTrackFound(TargetMatchingEventArgs e)
    {
        TrackFound?.Invoke(this, e);
        Handler?.Invoke(nameof(TrackFound), e);
    }

    /// <summary>
    /// Raises the TrackLost event with the provided event arguments.
    /// </summary>
    /// <param name="e">Event arguments containing information about the tracked target.</param>
    internal void OnTrackLost(EventArgs e)
    {
        TrackLost?.Invoke(this, e);
        Handler?.Invoke(nameof(TrackLost), e);
    }

    /// <summary>
    /// Applies the specified recognition module to enable target recognition.
    /// </summary>
    /// <param name="recognition">The recognition module to apply.</param>
    public void SetRecoService(IRecognition recognition)
    {
        Recognition = recognition;
    }

    #endregion
}
