using OpenVision.Core.Utils;

namespace OpenVision.Core.Reco.DataTypes;

/// <summary>
/// Represents an image with associated data and properties.
/// </summary>
public sealed class ImageData
{
    #region Properties

    /// <summary>
    /// Gets the unique identifier of the image.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the byte array representation of the image.
    /// </summary>
    public byte[] Buffer { get; }

    /// <summary>
    /// Gets the internal OpenCV Mat object that holds the image data.
    /// </summary>
    internal Mat Mat { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Private constructor to create an Image object.
    /// </summary>
    /// <param name="id">The unique identifier of the image.</param>
    /// <param name="mat">The OpenCV Mat object holding the image data.</param>
    /// <param name="buffer">The byte array representation of the image.</param>
    private ImageData(string id, Mat mat, byte[] buffer)
    {
        Id = id;
        Mat = mat;
        Buffer = buffer;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Loads an image from a file into an Image object.
    /// </summary>
    /// <param name="fileName">The path of the image file.</param>
    /// <returns>An Image object representing the loaded image.</returns>
    public static ImageData Load(string fileName)
    {
        var mat = fileName.FileToMat();
        var buffer = mat.ToArray();

#if ANDROID
        var width = mat.Width();
        var height = mat.Height();
#else
        var width = mat.Width;
        var height = mat.Height;
#endif
        var unitsX = 1;
        var unitsY = UnitsHelper.CalculateYUnits(unitsX, width, height);

        return new ImageData(fileName, mat, buffer);
    }

    /// <summary>
    /// Loads an image from a byte array into an Image object.
    /// </summary>
    /// <param name="buffer">The byte array containing the image data.</param>
    /// <returns>An Image object representing the loaded image.</returns>
    public static ImageData Load(byte[] buffer)
    {
        var mat = buffer.ToMat();
        var id = Guid.NewGuid().ToString();
#if ANDROID
        var width = mat.Width();
        var height = mat.Height();
#else
        var width = mat.Width;
        var height = mat.Height;
#endif
        var unitsX = 1;
        var unitsY = UnitsHelper.CalculateYUnits(unitsX, width, height);

        return new ImageData(id, mat, buffer);
    }

    /// <summary>
    /// Loads an image from a stream into an Image object.
    /// </summary>
    /// <param name="stream">The stream containing the image data.</param>
    /// <returns>An Image object representing the loaded image.</returns>
    public static ImageData Load(Stream stream)
    {
        byte[] buffer;

        if (stream.CanSeek)
        {
            buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
        }
        else
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            buffer = memoryStream.ToArray();
        }

        var mat = buffer.ToMat();
        var id = Guid.NewGuid().ToString();
#if ANDROID
        var width = mat.Width();
        var height = mat.Height();
#else
        var width = mat.Width;
        var height = mat.Height;
#endif
        var unitsX = 1;
        var unitsY = UnitsHelper.CalculateYUnits(unitsX, width, height);

        return new ImageData(id, mat, buffer);
    }

    #endregion
}