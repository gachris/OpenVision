using OpenVision.Api.Core;
using OpenVision.Api.Core.Util;
using OpenVision.Api.Target.Requests;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Api.Target.Resources;

/// <summary>
/// Represents a resource on PixelProdigy Vision for accessing targets for the current database.
/// </summary>
public class TargetListResource
{
    private readonly IClientService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetListResource"/> class.
    /// </summary>
    /// <param name="service">The client service used for making requests.</param>
    public TargetListResource(IClientService service) => _service = service;

    /// <summary>
    /// Creates a new <see cref="ListRequest"/> instance for retrieving the list of targets.
    /// </summary>
    /// <returns>A new <see cref="ListRequest"/> instance for retrieving the list of targets.</returns>
    public virtual ListRequest List() => new(_service);

    /// <summary>
    /// Creates a new <see cref="GetRequest"/> instance for retrieving a specific target by ID.
    /// </summary>
    /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
    /// <returns>A new <see cref="GetRequest"/> instance for retrieving a specific target by ID.</returns>
    public virtual GetRequest Get(string targetId) => new(_service, targetId);

    /// <summary>
    /// Creates a new <see cref="InsertRequest"/> instance that can insert a new target.
    /// </summary>            
    /// <param name="body">The target to insert.</param>
    /// <returns>A new <see cref="InsertRequest"/> instance that can insert a new target.</returns>
    public virtual InsertRequest Insert(PostTrackableRequest body) => new(_service, body);

    /// <summary>
    /// Creates a new <see cref="UpdateRequest"/> instance that can update an existing target.
    /// </summary>
    /// <param name="body">The body of the request containing the updated target information.</param>
    /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
    /// <returns>A new <see cref="UpdateRequest"/> instance that can update an existing target.</returns>
    public virtual UpdateRequest Update(UpdateTrackableRequest body, string targetId) => new(_service, body, targetId);

    /// <summary>
    /// Creates a new <see cref="DeleteRequest"/> instance that can delete an existing target by ID.
    /// </summary>
    /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
    /// <returns>A new <see cref="DeleteRequest"/> instance that can delete an existing target by ID.</returns>
    public virtual DeleteRequest Delete(string targetId) => new(_service, targetId);

    /// <summary>
    /// Represents a request to retrieve a list of targets.
    /// </summary>
    public class ListRequest : TargetBaseServiceRequest<GetAllTrackablesResponse>
    {
        /// <summary>
        /// Constructs a new <see cref="ListRequest"/> instance.
        /// </summary>
        /// <param name="service">The client service used for making requests.</param>
        public ListRequest(IClientService service) : base(service)
        {
        }

        /// <inheritdoc/>
        public override string MethodName => "list";

        /// <inheritdoc/>
        public override string HttpMethod => ApiMethodConstants.GET;

        /// <inheritdoc/>
        public override string RestPath => "api/ws";
    }

    /// <summary>
    /// Represents a request to get a single target by ID.
    /// </summary>
    public class GetRequest : TargetBaseServiceRequest<TrackableRetrieveResponse>
    {
        /// <summary>
        /// Constructs a new <see cref="GetRequest"/> instance.
        /// </summary>
        /// <param name="service">The client service used for making requests.</param>
        /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
        public GetRequest(IClientService service, string targetId) : base(service)
        {
            TargetId = targetId;
        }

        /// <summary>
        /// Gets the target ID.
        /// </summary>
        [RequestParameter("targetId", RequestParameterType.Path)]
        public virtual string TargetId { get; }

        /// <inheritdoc/>
        public override string MethodName => "get";

        /// <inheritdoc/>
        public override string HttpMethod => ApiMethodConstants.GET;

        /// <inheritdoc/>
        public override string RestPath => "api/ws/{targetId}";

        /// <inheritdoc/>
        protected override void InitParameters()
        {
            base.InitParameters();

            RequestParameters.Add("targetId", new Parameter("targetId", "path", true));
        }
    }

    /// <summary>
    /// Represents a request to insert a target.
    /// </summary>
    public class InsertRequest : TargetBaseServiceRequest<PostTrackableResponse>
    {
        /// <summary>
        /// Constructs a new <see cref="InsertRequest"/> instance.
        /// </summary>
        /// <param name="service">The client service used for making requests.</param>
        /// <param name="body">The target to insert.</param>
        public InsertRequest(IClientService service, PostTrackableRequest body) : base(service)
        {
            Body = body;
        }

        /// <summary>
        /// Gets the body of this request.
        /// </summary>
        private PostTrackableRequest Body { get; }

        /// <inheritdoc/>
        protected override object GetBody() => Body;

        /// <inheritdoc/>
        public override string MethodName => "insert";

        /// <inheritdoc/>
        public override string HttpMethod => ApiMethodConstants.POST;

        /// <inheritdoc/>
        public override string RestPath => "api/ws";
    }

    /// <summary>
    /// Represents a request to update an existing target.
    /// </summary>
    public class UpdateRequest : TargetBaseServiceRequest<UpdateTrackableResponse>
    {
        /// <summary>
        /// Constructs a new <see cref="UpdateRequest"/> instance.
        /// </summary>
        /// <param name="service">The client service used for making requests.</param>
        /// <param name="body">The body of the request containing the updated target information.</param>
        /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
        public UpdateRequest(IClientService service, UpdateTrackableRequest body, string targetId) : base(service)
        {
            TargetId = targetId;
            Body = body;
        }

        /// <summary>
        /// Gets the target ID.
        /// </summary>
        [RequestParameter("targetId", RequestParameterType.Path)]
        public virtual string TargetId { get; }

        /// <summary>
        /// Gets the body of this request.
        /// </summary>
        private UpdateTrackableRequest Body { get; set; }

        /// <inheritdoc/>
        protected override object GetBody() => Body;

        /// <inheritdoc/>
        public override string MethodName => "update";

        /// <inheritdoc/>
        public override string HttpMethod => ApiMethodConstants.PUT;

        /// <inheritdoc/>
        public override string RestPath => "api/ws/{targetId}";

        /// <inheritdoc/>
        protected override void InitParameters()
        {
            base.InitParameters();

            RequestParameters.Add("targetId", new Parameter("targetId", "path", true));
        }
    }

    /// <summary>
    /// Represents a request to delete a target.
    /// </summary>
    public class DeleteRequest : TargetBaseServiceRequest<DeleteTrackableResponse>
    {
        /// <summary>
        /// Constructs a new <see cref="DeleteRequest"/> instance.
        /// </summary>
        /// <param name="service">The client service used for making requests.</param>
        /// <param name="targetId">The ID of the target to retrieve. To retrieve target IDs, call the <see cref="List"/> method.</param>
        public DeleteRequest(IClientService service, string targetId) : base(service)
        {
            TargetId = targetId;
        }

        /// <summary>
        /// Gets the target ID.
        /// </summary>
        [RequestParameter("targetId", RequestParameterType.Path)]
        public virtual string TargetId { get; }

        /// <inheritdoc/>
        public override string MethodName => "delete";

        /// <inheritdoc/>
        public override string HttpMethod => ApiMethodConstants.DELETE;

        /// <inheritdoc/>
        public override string RestPath => "api/ws/{targetId}";

        /// <inheritdoc/>
        protected override void InitParameters()
        {
            base.InitParameters();

            RequestParameters.Add("targetId", new Parameter("targetId", "path", true));
        }
    }
}
