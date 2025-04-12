namespace OpenVision.Shared.Types;

/// <summary>
/// Represents the result code for an API operation.
/// </summary>
public enum ResultCode
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success,

    /// <summary>
    /// The request data was invalid.
    /// </summary>
    ValidationError,

    /// <summary>
    /// The request was invalid.
    /// </summary>
    InvalidRequest,

    /// <summary>
    /// The request was unauthorized.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The request was forbidden.
    /// </summary>
    Forbidden,

    /// <summary>
    /// The requested record was not found.
    /// </summary>
    RecordNotFound,

    /// <summary>
    /// An internal server error occurred.
    /// </summary>
    InternalServerError,

    /// <summary>
    /// An unknown error occurred.
    /// </summary>
    UnknownError
}

/// <summary>
/// Represents the result codes returned by the API.
/// </summary>
//public enum ResultCodeEnum
//{
//    /// <summary>
//    /// Transaction succeeded
//    /// </summary>
//    [Description("Transaction succeeded")]
//    Success   /*  OK (200)*/ 	,

//    /// <summary>
//    /// Target created (target POST response)
//    /// </summary>
//    [Description("Target created (target POST response)")]
//    TargetCreated   /*Created (201)*/ 	,

//    /// <summary>
//    /// Signature authentication failed
//    /// </summary>
//    [Description("Signature authentication failed")]
//    AuthenticationFailure   /*Authentication failure (401)*/,

//    /// <summary>
//    /// Request timestamp outside allowed range
//    /// </summary>
//    [Description("Request timestamp outside allowed range")]
//    RequestTimeTooSkewed    /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The corresponding target name already exists (target POST/PUT response)
//    /// </summary>
//    [Description("The corresponding target name already exists (target POST/PUT response)")]
//    TargetNameExist     /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The maximum number of API calls for this database has been reached.
//    /// </summary>
//    [Description("The maximum number of API calls for this database has been reached.")]
//    RequestQuotaReached     /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The target is in the processing state and cannot be updated.
//    /// </summary>
//    [Description("The target is in the processing state and cannot be updated.")]
//    TargetStatusProcessing  /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The request could not be completed because the target is not in the success state.
//    /// </summary>
//    [Description("The request could not be completed because the target is not in the success state.")]
//    TargetStatusNotSuccess  /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The maximum number of targets for this database has been reached.
//    /// </summary>
//    [Description("The maximum number of targets for this database has been reached.")]
//    TargetQuotaReached  /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The request could not be completed because this database has been suspended.
//    /// </summary>
//    [Description("The request could not be completed because this database has been suspended.")]
//    ProjectSuspended    /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The request could not be completed because this database is inactive.
//    /// </summary>
//    [Description("The request could not be completed because this database is inactive.")]
//    ProjectInactive     /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The request could not be completed because this database is not allowed to make API requests.
//    /// </summary>
//    [Description("The request could not be completed because this database is not allowed to make API requests.")]
//    ProjectHasNoApiAccess   /*Forbidden (403)*/ 	,

//    /// <summary>
//    /// The specified target ID does not exist (target PUT/GET/DELETE response)
//    /// </summary>
//    [Description("The specified target ID does not exist (target PUT/GET/DELETE response)")]
//    UnknownTarget   /*Not Found (404)*/ 	,

//    /// <summary>
//    /// Image corrupted or format not supported (target POST/PUT response)
//    /// </summary>
//    [Description("Image corrupted or format not supported (target POST/PUT response)")]
//    BadImage    /*Unprocessable Entity (422)*/ 	,

//    /// <summary>
//    /// Target metadata size exceeds maximum limit (target POST/PUT response)
//    /// </summary>
//    [Description("Target metadata size exceeds maximum limit (target POST/PUT response)")]
//    ImageTooLarge   /*Unprocessable Entity (422)*/ 	,

//    /// <summary>
//    /// Image size exceeds maximum limit (target POST/PUT response)
//    /// </summary>
//    [Description("Image size exceeds maximum limit (target POST/PUT response)")]
//    MetadataTooLarge    /*Unprocessable Entity (422)*/ 	,

//    /// <summary>
//    /// Start date is after the end date
//    /// </summary>
//    [Description("Start date is after the end date")]
//    DateRangeError  /*Unprocessable Entity (422)*/ 	,

//    /// <summary>
//    /// The request was invalid and could not be processed. Check the request headers and fields. | The server encountered an internal error; please retry the request
//    /// </summary>
//    [Description("The request was invalid and could not be processed. Check the request headers and fields. | The server encountered an internal error; please retry the request")]
//    Fail    /*Unprocessable Entity (422) | Internal Server Error (500)*/ 	,
//}