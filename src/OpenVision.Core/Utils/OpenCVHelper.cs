#if !ANDROID
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using OpenVision.Core.DataTypes;

namespace OpenVision.Core.Utils;

/// <summary>
/// Helper class containing extension methods and utilities for working with OpenCV.
/// </summary>
public static class OpenCVHelper
{
#if ANDROID
    /// <summary>
    /// Struct representing a keypoint in Android's OpenCV library.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct MKeyPoint
    {
        /// <summary>
        /// The location of the keypoint.
        /// </summary>
        public System.Drawing.PointF Point;

        /// <summary>
        /// Size of the keypoint.
        /// </summary>
        public float Size;

        /// <summary>
        /// Orientation of the keypoint.
        /// </summary>
        public float Angle;

        /// <summary>
        /// Response of the keypoint.
        /// </summary>
        public float Response;

        /// <summary>
        /// Octave of the keypoint.
        /// </summary>
        public int Octave;

        /// <summary>
        /// Class ID of the keypoint.
        /// </summary>
        public int ClassId;
    }
#endif

    private const int ransacReprojThreshold = 2;

    /// <summary>
    /// Extension method to check whether the given Mat object is empty.
    /// </summary>
    /// <param name="mat">The Mat object to check for emptiness.</param>
    /// <returns>True if the Mat object is empty; otherwise, false.</returns>
    public static bool IsEmpty(this Mat mat)
    {
#if ANDROID
        return mat.Empty();
#else
        return mat.IsEmpty;
#endif
    }

    /// <summary>
    /// Converts a byte array to an OpenCV Mat object.
    /// </summary>
    /// <param name="buffer">Byte array containing image data.</param>
    /// <returns>OpenCV Mat object.</returns>
    public static Mat ToMat(this byte[] buffer)
    {
#if ANDROID
        return Imgcodecs.Imdecode(new MatOfByte(buffer), Imgcodecs.ImreadUnchanged)!;
#else
        var mat = new Mat();
        CvInvoke.Imdecode(buffer, ImreadModes.Unchanged, mat);
        return mat;
#endif
    }

    /// <summary>
    /// Loads an image file into an OpenCV Mat object.
    /// </summary>
    /// <param name="fileName">Path to the image file.</param>
    /// <returns>OpenCV Mat object representing the image.</returns>
    public static Mat FileToMat(this string fileName)
    {
#if ANDROID
        var matOfByte = new MatOfByte();
        return Imgcodecs.Imread(fileName, Imgcodecs.ImreadUnchanged)!;
#else
        return CvInvoke.Imread(fileName, ImreadModes.Unchanged);
#endif
    }

    /// <summary>
    /// Converts a base64-encoded string representation of an image to an OpenCV Mat object.
    /// </summary>
    /// <param name="base64">Base64-encoded string representation of the image.</param>
    /// <returns>OpenCV Mat object representing the image.</returns>
    public static Mat Base64ToMat(this string base64)
    {
#if ANDROID
        var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(base64, string.Empty);
        var buffer = Convert.FromBase64String(imageData);
        return Imgcodecs.Imdecode(new MatOfByte(buffer), Imgcodecs.ImreadUnchanged)!;
#else
        var mat = new Mat();
        var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(base64, string.Empty);
        var buf = Convert.FromBase64String(imageData);
        CvInvoke.Imdecode(buf, ImreadModes.Unchanged, mat);
        return mat;
#endif
    }

    /// <summary>
    /// Converts a byte array representing keypoint data to an OpenCV MatOfKeyPoint object.
    /// </summary>
    /// <param name="bytes">Byte array containing keypoint data.</param>
    /// <returns>OpenCV MatOfKeyPoint object.</returns>
    public static MatOfKeyPoint KeyPointBytesToMatOfKeyPoint(this byte[] bytes)
    {
        return new(bytes.BytesToKeyPointArray());
    }

    /// <summary>
    /// Converts a byte array representing keypoint data to an array of KeyPoint objects.
    /// </summary>
    /// <param name="keyPointsArray">Byte array containing keypoint data.</param>
    /// <returns>Array of KeyPoint objects.</returns>
    public static KeyPoint[] BytesToKeyPointArray(this byte[] keyPointsArray)
    {
#if ANDROID
        var size = Marshal.SizeOf<MKeyPoint>();
        var arrayLength = keyPointsArray.Length / size;
        var mKeyPointArray = new KeyPoint[arrayLength];
        var start = Marshal.AllocHGlobal(keyPointsArray.Length);

        Marshal.Copy(keyPointsArray, 0, start, keyPointsArray.Length);

        for (var i = 0; i < arrayLength; i++)
        {
            var current = new nint(start.ToInt64() + i * size);
            var mKeyPoint = Marshal.PtrToStructure<MKeyPoint>(current);
            mKeyPointArray[i] = new KeyPoint(mKeyPoint.Point.X,
                                             mKeyPoint.Point.Y,
                                             mKeyPoint.Size,
                                             mKeyPoint.Angle,
                                             mKeyPoint.Response,
                                             mKeyPoint.Octave);
        }

        Marshal.FreeHGlobal(start);

        return mKeyPointArray;
#else
        var size = Marshal.SizeOf<KeyPoint>();
        var arrayLength = keyPointsArray.Length / size;
        var mKeyPointArray = new KeyPoint[arrayLength];
        var start = Marshal.AllocHGlobal(keyPointsArray.Length);

        Marshal.Copy(keyPointsArray, 0, start, keyPointsArray.Length);

        for (var i = 0; i < arrayLength; i++)
        {
            var current = new nint(start.ToInt64() + i * size);
            mKeyPointArray[i] = Marshal.PtrToStructure<KeyPoint>(current);
        }

        Marshal.FreeHGlobal(start);

        return mKeyPointArray;
#endif
    }

    /// <summary>
    /// Converts a byte array representing descriptor data to an OpenCV Mat object.
    /// </summary>
    /// <param name="bytes">Byte array containing descriptor data.</param>
    /// <param name="rows">Number of rows in the resulting Mat.</param>
    /// <param name="cols">Number of columns in the resulting Mat.</param>
    /// <returns>OpenCV Mat object containing descriptors.</returns>
    public static Mat DescriptorBytesToMat(this byte[] bytes, int rows, int cols)
    {
#if ANDROID
        var descriptorMat = new Mat(rows, cols, CvType.Cv32f);

        // Reverse the byte order of the descriptors to match the little-endian byte order on Android
        var descriptorBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i += sizeof(float))
        {
            var floatBytes = new byte[sizeof(float)];
            Array.Copy(bytes, i, floatBytes, 0, sizeof(float));
            Array.Reverse(floatBytes);
            Array.Copy(floatBytes, 0, descriptorBytes, i, sizeof(float));
        }

        // Convert the byte array to a ByteBuffer
        var byteBuffer = Java.Nio.ByteBuffer.Wrap(descriptorBytes);

        // Convert the ByteBuffer to a float array
        var floatArray = new float[descriptorBytes.Length / sizeof(float)];
        byteBuffer.AsFloatBuffer().Get(floatArray);

        // Put the float data into the Mat
        descriptorMat.Put(0, 0, floatArray);
#else
        var descriptorMat = new Mat(rows, cols, DepthType.Cv32F, 1);
        Marshal.Copy(bytes, 0, descriptorMat.DataPointer, bytes.Length);
#endif
        return descriptorMat;
    }

    /// <summary>
    /// Converts an OpenCV Mat object to a byte array.
    /// </summary>
    /// <param name="mat">OpenCV Mat object to convert.</param>
    /// <returns>Byte array containing image data.</returns>
    public static byte[] ToArray(this Mat mat)
    {
#if ANDROID
        var matOfByte = new MatOfByte();
        Imgcodecs.Imencode(".jpg", mat, matOfByte);
        return matOfByte.ToArray()!;
#else
        return CvInvoke.Imencode(".jpg", mat);
#endif
    }

    /// <summary>
    /// Converts an OpenCV MatOfKeyPoint object to a byte array.
    /// </summary>
    /// <param name="matOfKeyPoint">OpenCV MatOfKeyPoint object to convert.</param>
    /// <returns>Byte array containing keypoint data.</returns>
    public static byte[] ToByteArray(this MatOfKeyPoint matOfKeyPoint)
    {
#if ANDROID
        var keypointsArray = matOfKeyPoint.ToArray()!;
        var keypointsByteArray = new byte[keypointsArray.Length * Marshal.SizeOf<MKeyPoint>()];
        var handle = GCHandle.Alloc(keypointsByteArray, GCHandleType.Pinned);
        var start = handle.AddrOfPinnedObject();
        for (var i = 0; i < keypointsArray.Length; i++)
        {
            var keypoint = keypointsArray[i]!;
            var mKeyPoint = new MKeyPoint()
            {
                Size = keypoint.Size,
                Octave = keypoint.Octave,
                Response = keypoint.Response,
                Point = new System.Drawing.PointF((float)keypoint.Pt!.X, (float)keypoint.Pt!.Y),
                Angle = keypoint.Angle
            };
            Marshal.StructureToPtr(mKeyPoint, start + i * Marshal.SizeOf<MKeyPoint>(), false);
        }
        handle.Free();

        return keypointsByteArray;
#else
        var keypointsArray = new byte[matOfKeyPoint.Size * Marshal.SizeOf<KeyPoint>()];
        var handle = GCHandle.Alloc(keypointsArray, GCHandleType.Pinned);
        var start = handle.AddrOfPinnedObject();
        for (var i = 0; i < matOfKeyPoint.Size; i++)
        {
            Marshal.StructureToPtr(matOfKeyPoint[i], start + i * Marshal.SizeOf<KeyPoint>(), false);
        }
        handle.Free();

        return keypointsArray;
#endif
    }

    /// <summary>
    /// Converts an OpenCV Mat object containing descriptors to a byte array.
    /// </summary>
    /// <param name="descriptorMat">OpenCV Mat object containing descriptors.</param>
    /// <returns>Byte array containing descriptor data.</returns>
    public static byte[] DescriptorToArray(this Mat descriptorMat)
    {
#if ANDROID
        // assume you have a Mat object named "descriptorMat" containing descriptors
        var numDescriptors = descriptorMat.Rows();
        var descriptorSize = descriptorMat.Cols();
        var bytesPerDescriptor = descriptorSize * sizeof(float);

        // create a one-dimensional array to hold the descriptors
        var descriptorArray = new float[numDescriptors * descriptorSize];

        // copy the data from the Mat to the array
        descriptorMat.Get(0, 0, descriptorArray);

        // create a new byte array to hold the descriptors
        var descriptorBytes = new byte[numDescriptors * bytesPerDescriptor];

        // copy the data from the float array to the byte array
        Buffer.BlockCopy(descriptorArray, 0, descriptorBytes, 0, descriptorBytes.Length);

        return descriptorBytes;
#else
        // assume you have a Mat object named "descriptorMat" containing descriptors
        var numDescriptors = descriptorMat.Rows;
        var descriptorSize = descriptorMat.Cols;
        var bytesPerDescriptor = descriptorSize * sizeof(float);

        // create a new byte array to hold the descriptors
        var descriptorBytes = new byte[numDescriptors * bytesPerDescriptor];

        // copy the data from the Mat to the byte array
        Marshal.Copy(descriptorMat.DataPointer, descriptorBytes, 0, descriptorBytes.Length);

        return descriptorBytes;
#endif
    }

    /// <summary>
    /// Converts an image to grayscale.
    /// </summary>
    /// <param name="image">Image to convert to grayscale.</param>
    public static void ToGray(this Mat image)
    {
#if ANDROID
        Imgproc.CvtColor(image, image, Imgproc.ColorBgr2gray);
#else
        CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Gray);
#endif
    }

    /// <summary>
    /// Applies Gaussian blur to the image.
    /// </summary>
    /// <param name="image">Image to blur.</param>
    /// <param name="kSize">Size of the Gaussian kernel.</param>
    /// <param name="sigmaX">Standard deviation in X direction.</param>
    public static void GaussianBlur(this Mat image, System.Drawing.Size kSize, double sigmaX)
    {
#if ANDROID
        Imgproc.GaussianBlur(image,
                             image,
                             new Size(kSize.Width, kSize.Height),
                             sigmaX);
#else
        CvInvoke.GaussianBlur(image,
                              image,
                              kSize,
                              sigmaX);
#endif
    }

    /// <summary>
    /// Resizes the image to fit within a maximum height or width while preserving aspect ratio.
    /// </summary>
    /// <param name="image">Image to resize.</param>
    /// <param name="maxHeightOrWidth">Maximum height or width of the resized image.</param>
    public static void LowResolution(this Mat image, double maxHeightOrWidth)
    {
#if ANDROID
        var currentHeight = image.Height();
        var currentWidth = image.Width();
        var scale = 1.0;

        if (currentHeight > maxHeightOrWidth || currentWidth > maxHeightOrWidth)
        {
            // Determine the scale factor to use for resizing
            scale = (double)maxHeightOrWidth / Math.Max(currentHeight, currentWidth);
        }

        // Resize the image using the determined scale factor
        var newSize = new Size((int)(currentWidth * scale), (int)(currentHeight * scale));
        Imgproc.Resize(image,
                       image,
                       newSize);
#else
        var currentHeight = image.Height;
        var currentWidth = image.Width;
        var scale = 1.0;

        if (currentHeight > maxHeightOrWidth || currentWidth > maxHeightOrWidth)
        {
            // Determine the scale factor to use for resizing
            scale = (double)maxHeightOrWidth / Math.Max(currentHeight, currentWidth);
        }

        // Resize the image using the determined scale factor
        var newSize = new Size((int)(currentWidth * scale), (int)(currentHeight * scale));
        CvInvoke.Resize(image, image, newSize);
#endif
    }

    /// <summary>
    /// Selects a Region of Interest (ROI) from the image.
    /// </summary>
    /// <param name="image">Image from which to select ROI.</param>
    /// <param name="roiX">X-coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiY">Y-coordinate of the top-left corner of the ROI.</param>
    /// <param name="roiWidth">Width of the ROI.</param>
    /// <param name="roiHeight">Height of the ROI.</param>
    public static void Roi(this Mat image, int roiX, int roiY, int roiWidth, int roiHeight)
    {
#if ANDROID
        var maxWidth = image.Cols() - roiX; // Maximum width of the ROI
        var maxHeight = image.Rows() - roiY; // Maximum height of the ROI
                                             // Adjust the ROI dimensions if they exceed the maximum values
        if (roiWidth > maxWidth)
        {
            roiWidth = maxWidth;
        }
        if (roiHeight > maxHeight)
        {
            roiHeight = maxHeight;
        }

        var roi = new Rect(roiX, roiY, roiWidth, roiHeight);
        image = new Mat(image, roi);
#else
        var maxWidth = image.Cols - roiX; // Maximum width of the ROI
        var maxHeight = image.Rows - roiY; // Maximum height of the ROI
                                           // Adjust the ROI dimensions if they exceed the maximum values
        if (roiWidth > maxWidth)
        {
            roiWidth = maxWidth;
        }
        if (roiHeight > maxHeight)
        {
            roiHeight = maxHeight;
        }

        var roi = new Rectangle(roiX, roiY, roiWidth, roiHeight);
        image = new Mat(image, roi);
#endif
    }

#if ANDROID
    public static void VoteForUniqueness(
        MatOfDMatch matches,
        double uniquenessThreshold,
        Mat mask)
    {
        var matchArray = matches.ToArray()!;
        int totalMatches = matchArray.Length;

        // Iterate through the matches
        for (int i = 0; i < totalMatches; i++)
        {
            // Get the first and second matches
            DMatch firstMatch = matchArray[i]; // Best match
            DMatch secondMatch = matchArray[i + 1]; // Second-best match

            // Ensure there is a second match
            if (secondMatch.Distance > 0)
            {
                // Calculate the distance ratio
                double distanceRatio = firstMatch.Distance / secondMatch.Distance;

                // If the distance ratio is below the threshold, mark as invalid
                if (distanceRatio < uniquenessThreshold)
                {
                    mask.Put(i, 0, new float[] { 0 }); // Mark as invalid (0)
                }
            }
            else
            {
                // If there's no valid second match, mark as invalid
                mask.Put(i, 0, new float[] { 0 }); // Mark as invalid (0)
            }
        }
    }

    public static int VoteForSizeAndOrientation(
        MatOfKeyPoint trainKeypoints,
        MatOfKeyPoint queryKeypoints,
        MatOfDMatch matches,
        Mat mask,
        double scaleIncrement,
        int rotationBins)
    {
        // Convert matches to an array for easy access
        var matchArray = matches.ToArray()!;
        var totalMatches = matchArray.Length;

        // Pre-calculate arrays for sizes and angles
        var trainKeyPointsArray = trainKeypoints.ToArray()!;
        var queryKeyPointsArray = queryKeypoints.ToArray()!;

        // Initialize a fixed-size array for votes
        var voteCount = new int[rotationBins];
        var scaleSums = new float[rotationBins];

        // Iterate through each match
        for (var i = 0; i < totalMatches; i++)
        {
            var queryMatch = matchArray[i]; // Best match

            // Get keypoints directly
            var queryKeyPoint = queryKeyPointsArray[queryMatch.QueryIdx];
            var trainKeyPoint = trainKeyPointsArray[queryMatch.TrainIdx];

            // Calculate scale and orientation
            var scale = trainKeyPoint.Size / queryKeyPoint.Size;
            var angle = trainKeyPoint.Angle - queryKeyPoint.Angle;

            // Normalize angle to [0, 360)
            angle = angle < 0 ? angle + 360 : angle;

            // Create a bin for the angle
            var angleBin = (int)(angle / (360f / rotationBins));
            angleBin = Math.Clamp(angleBin, 0, rotationBins - 1);

            // Update vote counts and scale sums
            scaleSums[angleBin] += scale;
            voteCount[angleBin]++;
        }

        // Determine the best scale and orientation pair
        var bestAngleBin = -1;
        var bestScale = 0f;
        var maxVotes = 0;

        for (var i = 0; i < rotationBins; i++)
        {
            if (voteCount[i] > maxVotes)
            {
                maxVotes = voteCount[i];
                bestAngleBin = i;
                bestScale = scaleSums[i] / maxVotes; // Average scale for this bin
            }
        }

        // If no valid votes, return
        if (maxVotes == 0) return 0;

        // Create a new mask for valid matches based on the best vote
        var validVotes = 0;

        for (var i = 0; i < totalMatches; i++)
        {
            var queryMatch = matchArray[i]; // Get best match

            // Get keypoints again
            var queryKeyPoint = queryKeyPointsArray[queryMatch.QueryIdx];
            var trainKeyPoint = trainKeyPointsArray[queryMatch.TrainIdx];

            // Calculate scale and angle for the current match
            var currentScale = trainKeyPoint.Size / queryKeyPoint.Size;
            var currentAngle = trainKeyPoint.Angle - queryKeyPoint.Angle;

            // Normalize angle to [0, 360)
            currentAngle = currentAngle < 0 ? currentAngle + 360 : currentAngle;

            // Calculate the angle bin for the current match
            var currentAngleBin = (int)(currentAngle / (360f / rotationBins));
            currentAngleBin = Math.Clamp(currentAngleBin, 0, rotationBins - 1);

            // Check if the current match is close to the best scale and angle
            if (Math.Abs(currentScale - bestScale) < scaleIncrement && currentAngleBin == bestAngleBin)
            {
                mask.Put(i, 0, new byte[] { 255 }); // Mark valid
                validVotes++;
            }
            else
            {
                mask.Put(i, 0, new byte[] { 0 }); // Mark invalid
            }
        }

        // Return the number of valid matches
        return validVotes;
    }
#endif

#if ANDROID
    /// <summary>
    /// Computes the homography matrix.
    /// </summary>
    public static Mat GetHomography(TargetMatchQuery queryInfo, TargetMatchQuery trainInfo, IList<DMatch> matches, Mat mask)
    {
        var listOfKeypointsObject = queryInfo.Keypoints.ToArray()!;
        var listOfKeypointsScene = trainInfo.Keypoints.ToArray()!;

        var objPoints = new MatOfPoint2f();
        var scenePoints = new MatOfPoint2f();

        var obj = new List<Point>();
        var scene = new List<Point>();

        foreach (var match in matches)
        {
            if (match.QueryIdx < listOfKeypointsObject.Length && match.TrainIdx < listOfKeypointsScene.Length)
            {
                obj.Add(listOfKeypointsObject[match.QueryIdx].Pt!);
                scene.Add(listOfKeypointsScene[match.TrainIdx].Pt!);
            }
        }

        // Convert lists to MatOfPoint2f
        objPoints.FromList(obj);
        scenePoints.FromList(scene);

        // Calculate the homography matrix
        return Calib3d.FindHomography(scenePoints, objPoints, Calib3d.Ransac, ransacReprojThreshold, mask)!;
    }
#else
    /// <summary>
    /// Computes the homography matrix.
    /// </summary>
    public static Mat GetHomography(TargetMatchQuery queryInfo, TargetMatchQuery trainInfo, VectorOfVectorOfDMatch matches, Mat mask)
    {
        return Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(trainInfo.Keypoints, queryInfo.Keypoints, matches, mask, ransacReprojThreshold);
    }
#endif
}