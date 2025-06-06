﻿namespace OpenVision.Api.Core;

/// <summary>
/// An attribute which is used to specially mark a property for reflective purposes,
/// assign a name to the property and indicate it's location in the request as either
/// in the path or query portion of the request URL.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequestParameterAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the parameter, Path or Query.
    /// </summary>
    public RequestParameterType Type { get; }

    /// <summary>
    /// Constructs a new property attribute to be a part of a REST URI.
    /// This constructor uses <see cref="RequestParameterType.Query" /> as the parameter's type.
    /// </summary>
    /// <param name="name">
    /// The name of the parameter. If the parameter is a path parameter this name will be used to substitute the
    /// string value into the path, replacing {name}. If the parameter is a query parameter, this parameter will be
    /// added to the query string, in the format "name=value".
    /// </param>
    public RequestParameterAttribute(string name) : this(name, RequestParameterType.Query)
    {
    }

    /// <summary>Constructs a new property attribute to be a part of a REST URI.</summary>
    /// <param name="name">
    /// The name of the parameter. If the parameter is a path parameter this name will be used to substitute the
    /// string value into the path, replacing {name}. If the parameter is a query parameter, this parameter will be
    /// added to the query string, in the format "name=value".
    /// </param>
    /// <param name="type">The type of the parameter, either Path or Query.</param>
    public RequestParameterAttribute(string name, RequestParameterType type)
    {
        Name = name;
        Type = type;
    }
}