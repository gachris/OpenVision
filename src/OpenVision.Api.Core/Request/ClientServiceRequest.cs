﻿using OpenVision.Api.Core.Extensions;
using OpenVision.Api.Core.Logger;
using OpenVision.Api.Core.Util;

namespace OpenVision.Api.Core.Request;

/// <summary>
/// Represents an abstract, strongly typed request base class to make requests to a service.
/// Supports a strongly typed response.
/// </summary>
/// <typeparam name="TResponse">The type of the response object</typeparam>
public abstract class ClientServiceRequest<TResponse> : IClientServiceRequest<TResponse>, IClientServiceRequest
{
    /// <summary>
    /// The class logger.
    /// </summary>
    private static readonly ILogger _logger = ApplicationContext.Logger.ForType<ClientServiceRequest<TResponse>>();

    private readonly Dictionary<string, IParameter> _requestParameters = new();

    /// <summary>
    /// The service on which this request will be executed.
    /// </summary>
    private readonly IClientService _service;

    /// <inheritdoc/>
    public abstract string MethodName { get; }

    /// <inheritdoc/>
    public abstract string RestPath { get; }

    /// <inheritdoc/>
    public abstract string HttpMethod { get; }

    /// <inheritdoc/>
    public IDictionary<string, IParameter> RequestParameters => _requestParameters;

    /// <inheritdoc/>
    public IClientService Service => _service;

    /// <summary>
    /// Creates a new service request.
    /// </summary>
    protected ClientServiceRequest(IClientService service)
    {
        _service = service;
        InitParameters();
    }

    /// <summary>
    /// Initializes request's parameters.
    /// </summary>
    protected virtual void InitParameters()
    {
    }

    /// <inheritdoc/>
    public TResponse Execute()
    {
        try
        {
            using var result = ExecuteUnparsedAsync(CancellationToken.None).Result;
            return ParseResponse(result).Result;
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
        catch
        {
            throw;
        }
    }

    /// <inheritdoc/>
    public Stream ExecuteAsStream()
    {
        try
        {
            return ExecuteUnparsedAsync(CancellationToken.None).Result.Content.ReadAsStreamAsync().Result;
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
        catch
        {
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TResponse> ExecuteAsync() => await ExecuteAsync(CancellationToken.None).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<TResponse> ExecuteAsync(CancellationToken cancellationToken)
    {
        TResponse response;
        using (var httpResponseMessage = await ExecuteUnparsedAsync(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            response = await ParseResponse(httpResponseMessage).ConfigureAwait(false);
        }
        return response;
    }

    /// <inheritdoc/>
    public async Task<Stream> ExecuteAsStreamAsync() => await ExecuteAsStreamAsync(CancellationToken.None).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Stream> ExecuteAsStreamAsync(CancellationToken cancellationToken)
    {
        var httpResponseMessage = await ExecuteUnparsedAsync(cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Sync executes the request without parsing the result. 
    /// </summary>
    private async Task<HttpResponseMessage> ExecuteUnparsedAsync(CancellationToken cancellationToken)
    {
        using var request = CreateRequest();
        return await _service.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses the response and deserialize the content into the requested response object.
    /// </summary>
    protected virtual async Task<TResponse> ParseResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await _service.DeserializeResponse<TResponse>(response).ConfigureAwait(false);
        }

        var requestError = await _service.DeserializeError(response).ConfigureAwait(false);
        throw new ApiResponseException(_service.Name, requestError, response.StatusCode);
    }

    /// <inheritdoc/>
    public HttpRequestMessage CreateRequest()
    {
        var request = CreateBuilder().CreateRequest();
        var body = GetBody();
        request.SetRequestSerailizedContent(_service, body);
        return request;
    }

    /// <summary>
    /// Creates the <see cref="RequestBuilder" /> which is used to generate a request.
    /// </summary>
    /// <returns>
    /// A new builder instance which contains the HTTP method and the right Uri with its path and query parameters.
    /// </returns>
    private RequestBuilder CreateBuilder()
    {
        var requestBuilder = new RequestBuilder(new Uri(Service.BaseUri), RestPath, HttpMethod);
        var parameterDictionary = ParameterUtils.CreateParameterDictionary(this);
        AddParameters(requestBuilder, ParameterCollection.FromDictionary(parameterDictionary));
        return requestBuilder;
    }

    ///<summary>Returns the body of the request.</summary>
    protected virtual object GetBody() => null;

    /// <summary>
    /// Adds path and query parameters to the given <c>requestBuilder</c>.
    /// </summary>
    private void AddParameters(RequestBuilder requestBuilder, ParameterCollection inputParameters)
    {
        foreach (KeyValuePair<string, string> inputParameter in inputParameters)
        {
            if (!RequestParameters.TryGetValue(inputParameter.Key, out var parameter))
                throw new ApiServiceException(Service.Name, string.Format("Invalid parameter \"{0}\" was specified", inputParameter.Key));

            var defaultValue = inputParameter.Value;

            if (!ParameterValidator.ValidateParameter(parameter, defaultValue))
                throw new ApiServiceException(Service.Name, string.Format("Parameter validation failed for \"{0}\"", parameter.Name));

            defaultValue ??= parameter.DefaultValue;

            var parameterType = parameter.ParameterType;

            if (!(parameterType == "path"))
            {
                if (parameterType == "query")
                {
                    if (!Equals(defaultValue, parameter.DefaultValue) || parameter.IsRequired)
                        requestBuilder.AddParameter(RequestParameterType.Query, inputParameter.Key, defaultValue);
                }
                else throw new ApiServiceException(_service.Name,
                                                   string.Format("Unsupported parameter type \"{0}\" for \"{1}\"",
                                                                 parameter.ParameterType,
                                                                 parameter.Name));
            }
            else requestBuilder.AddParameter(RequestParameterType.Path, inputParameter.Key, defaultValue);
        }

        foreach (IParameter parameter in RequestParameters.Values)
        {
            if (parameter.IsRequired && !inputParameters.ContainsKey(parameter.Name))
                throw new ApiServiceException(_service.Name, string.Format("Parameter \"{0}\" is missing", parameter.Name));
        }
    }
}
