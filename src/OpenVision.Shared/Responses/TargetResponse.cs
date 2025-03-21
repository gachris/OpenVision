using System.Text.Json.Serialization;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents the response payload returned by the API for a target record.
/// </summary>
public class TargetResponse
{
    /// <summary>
    /// Gets the unique identifier of the target.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual Guid Id { get; }

    /// <summary>
    /// Gets the unique identifier of the database that the target belongs to.
    /// </summary>
    [JsonPropertyName("database_id")]
    public virtual Guid DatabaseId { get; }

    /// <summary>
    /// Gets the preprocessed image of the target.
    /// </summary>
    [JsonPropertyName("preprocess_image")]
    public virtual byte[] PreprocessImage { get; }

    /// <summary>
    /// Gets the image of the target after it has been processed.
    /// </summary>
    [JsonPropertyName("after_process_image")]
    public virtual byte[] AfterProcessImage { get; }

    /// <summary>
    /// Gets the image of the target after it has been processed with keypoints.
    /// </summary>
    [JsonPropertyName("after_process_image_with_keypoints")]
    public virtual byte[] AfterProcessImageWithKeypoints { get; }

    /// <summary>
    /// Gets the units in the x direction.
    /// </summary>
    [JsonPropertyName("x_units")]
    public virtual float XUnits { get; }

    /// <summary>
    /// Gets the units in the y direction.
    /// </summary>
    [JsonPropertyName("y_units")]
    public virtual float YUnits { get; }

    /// <summary>
    /// Gets the number of recognitions.
    /// </summary>
    [JsonPropertyName("recos")]
    public virtual int Recos { get; }

    /// <summary>
    /// Gets the rating of the target.
    /// </summary>
    [JsonPropertyName("rating")]
    public virtual int Rating { get; }

    /// <summary>
    /// Gets the name of the target.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string Name { get; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    [JsonPropertyName("type")]
    public virtual TargetType Type { get; }

    /// <summary>
    /// Gets the active flag of the target.
    /// </summary>
    [JsonPropertyName("status")]
    public virtual ActiveFlag ActiveFlag { get; }

    /// <summary>
    /// Gets the metadata of the target.
    /// </summary>
    [JsonPropertyName("metadata")]
    public virtual string Metadata { get; }

    /// <summary>
    /// Gets the creation date of the target object.
    /// </summary>
    [JsonPropertyName("created")]
    public virtual DateTimeOffset Created { get; }

    /// <summary>
    /// Gets the last update date of the target object.
    /// </summary>
    [JsonPropertyName("updated")]
    public virtual DateTimeOffset Updated { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetResponse"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the target record.</param>
    /// <param name="databaseId">The unique identifier of the database that the target record belongs to.</param>
    /// <param name="preprocessImage">The binary data of the pre-processed image associated with the target record.</param>
    /// <param name="afterProcessImage">The binary data of the image after processing associated with the target record.</param>
    /// <param name="afterProcessImageWithKeypoints">The binary data of the image after processing with keypoints associated with the target record.</param>
    /// <param name="xUnits">The X-axis units of the target record.</param>
    /// <param name="yUnits">The Y-axis units of the target record.</param>
    /// <param name="recos">The number of recognitions of the target record.</param>
    /// <param name="rating">The rating of the target record.</param>
    /// <param name="name">The name of the target record.</param>
    /// <param name="type">The type of the target record.</param>
    /// <param name="activeFlag">The active flag of the target record.</param>
    /// <param name="metadata">The metadata of the target record.</param>
    /// <param name="created">The date and time when the target record was created.</param>
    /// <param name="updated">The date and time when the target record was last updated.</param>
    public TargetResponse(Guid id,
                          Guid databaseId,
                          byte[] preprocessImage,
                          byte[] afterProcessImage,
                          byte[] afterProcessImageWithKeypoints,
                          float xUnits,
                          float yUnits,
                          int recos,
                          int rating,
                          string name,
                          TargetType type,
                          ActiveFlag activeFlag,
                          string metadata,
                          DateTimeOffset created,
                          DateTimeOffset updated)
    {
        Id = id;
        DatabaseId = databaseId;
        PreprocessImage = preprocessImage;
        AfterProcessImage = afterProcessImage;
        AfterProcessImageWithKeypoints = afterProcessImageWithKeypoints;
        XUnits = xUnits;
        YUnits = yUnits;
        Recos = recos;
        Rating = rating;
        Name = name;
        Type = type;
        ActiveFlag = activeFlag;
        Metadata = metadata;
        Created = created;
        Updated = updated;
    }
}