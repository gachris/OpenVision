using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Configuration;
using OpenVision.Server.Core.Mappers;
using OpenVision.Server.Core.Services;
using OpenVision.Server.Core.Utils;
using OpenVision.Server.EntityFramework.DbContexts;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for configuring and enhancing a WebApplicationBuilder in ASP.NET Core.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds default services and configurations for an API application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <param name="connectionString">Database connection string.</param>
    /// <param name="databaseProviderType">Database provider type.</param>
    /// <returns>The WebApplicationBuilder instance.</returns>
    public static IHostApplicationBuilder AddApiServiceDefaults(this WebApplicationBuilder builder, ApiConfiguration apiConfiguration, string connectionString, DatabaseProviderType databaseProviderType)
    {
        // Add AutoMapper with MappingProfile
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // Register API configuration as singleton
        builder.Services.AddSingleton(apiConfiguration);

        // Add transient services
        builder.Services.AddTransient<IWebServerService, WebServerService>();
        builder.Services.AddTransient<IDatabasesService, DatabasesService>();
        builder.Services.AddTransient<IFilesService, FilesService>();
        builder.Services.AddTransient<ITargetsService, TargetsService>();
        builder.Services.AddTransient<IApiKeyGeneratorService, ApiKeyGeneratorService>();

        // Add DbContext based on database provider
        builder.Services.AddDbContext(connectionString, databaseProviderType);

        // Add HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // Add UriService
        builder.Services.AddUriService();

        // Add authentication services
        builder.Services.AddAuthentication(apiConfiguration);

        // Add authorization policies
        builder.Services.AddAuthorizationPolicy(apiConfiguration);

        // Add CORS policies
        builder.Services.AddCors(apiConfiguration);

        // Add API controllers
        builder.Services.AddApiControllers();

        // Add API explorer endpoints
        builder.Services.AddEndpointsApiExplorer();

        // Add Swagger generation
        builder.Services.AddSwaggerGen(apiConfiguration);

        return builder;
    }

    /// <summary>
    /// Adds DbContext configuration based on the database provider type.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="connectionString">Database connection string.</param>
    /// <param name="databaseProviderType">Database provider type.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString, DatabaseProviderType databaseProviderType)
    {
        var migrationAssembly = MigrationAssemblyConfiguration.GetMigrationAssemblyByProvider(databaseProviderType);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            switch (databaseProviderType)
            {
                case DatabaseProviderType.MySql:
                    options.UseMySQL(connectionString, mySqlOptions =>
                    {
                        mySqlOptions.MigrationsAssembly(migrationAssembly);
                        mySqlOptions.EnableRetryOnFailure();
                    });
                    break;
                case DatabaseProviderType.PostgreSQL:
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(migrationAssembly);
                        npgsqlOptions.EnableRetryOnFailure();
                    });
                    break;
                case DatabaseProviderType.SqlServer:
                    options.UseSqlServer(connectionString, sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly(migrationAssembly);
                        sqlServerOptions.EnableRetryOnFailure();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });

        return services;
    }

    /// <summary>
    /// Configures authentication services.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddAuthentication(this IServiceCollection services, ApiConfiguration apiConfiguration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = apiConfiguration.Authority;
            options.RequireHttpsMetadata = apiConfiguration.RequireHttpsMetadata;
            options.Audience = apiConfiguration.Audience;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        })
        .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, null);

        return services;
    }

    /// <summary>
    /// Adds authorization policies based on API configuration settings.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services, ApiConfiguration apiConfiguration)
    {
        services.AddAuthorization(options =>
        {
            // Bearer token policy
            options.AddPolicy(AuthorizationConsts.BearerPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireScope();
                foreach (var scope in apiConfiguration.Scopes)
                {
                    policy.RequireClaim("scope", scope);
                }
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            });

            // Client API key policy
            options.AddPolicy(AuthorizationConsts.ClientApiKeyPolicy,
                policy => policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(c => c.Type == ApiKeyDefaults.X_API_KEY)
                           && context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Client");
                })
                .AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme));

            // Server API key policy
            options.AddPolicy(AuthorizationConsts.ServerApiKeyPolicy,
                policy => policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(c => c.Type == ApiKeyDefaults.X_API_KEY)
                           && context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Server");
                })
                .AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme));
        });

        return services;
    }

    /// <summary>
    /// Adds a singleton UriService instance.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddUriService(this IServiceCollection services)
    {
        services.AddSingleton<IUriService>(serviceProvider =>
        {
            var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>() ?? throw new InvalidOperationException("IHttpContextAccessor is not registered.");
            var context = accessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
            var request = context.Request;
            var uri = $"{request.Scheme}://{request.Host.ToUriComponent()}";

            return new UriService(uri);
        });

        return services;
    }

    /// <summary>
    /// Adds CORS policies based on API configuration settings.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddCors(this IServiceCollection services, ApiConfiguration apiConfiguration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    if (apiConfiguration.CorsAllowAnyOrigin)
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(apiConfiguration.CorsAllowOrigins);
                    }

                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });

        return services;
    }

    /// <summary>
    /// Adds API controllers with customized JSON serialization settings.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(new ValidateModelFilter());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        return services;
    }

    /// <summary>
    /// Adds Swagger generation with OAuth2 security definitions.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddSwaggerGen(this IServiceCollection services, ApiConfiguration apiConfiguration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiConfiguration.ApiVersion, new OpenApiInfo { Title = apiConfiguration.ApiName, Version = apiConfiguration.ApiVersion });

            // Define OAuth2 security scheme for Swagger UI
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{apiConfiguration.Authority}/connect/authorize"),
                        TokenUrl = new Uri($"{apiConfiguration.Authority}/connect/token"),
                        Scopes = apiConfiguration.Scopes.ToDictionary(x => x, y => y)
                    }
                }
            });

            // Add custom operation filter for authorization checks
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    /// <summary>
    /// Configures application middleware and pipeline for the API application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The WebApplication instance.</returns>
    public static IApplicationBuilder AddApplicationDefaults(this WebApplication app, ApiConfiguration apiConfiguration)
    {
        // Configure WebSocket options
        var webSocketOptions = new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };

        app.UseWebSockets(webSocketOptions);

        // Add headers forwarding configuration
        app.AddForwardHeaders();

        // Configure development environment settings
        app.ConfigureDevelopmentEnvironment();

        // Use Swagger and Swagger UI
        app.UseSwagger(apiConfiguration);

        // Configure exception handling middleware
        app.ConfigureExceptionHandler();

        // Enable CORS
        app.UseCors();

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        // Enable routing
        app.UseRouting();

        // Use authentication middleware
        app.UseAuthentication();

        // Use custom challenge middleware
        app.UseMiddleware<ChallengeMiddleware>();

        // Use authorization middleware
        app.UseAuthorization();

        // Map API controllers
        app.MapControllers();

        // Migrate database
        app.MigrateDatabase();

        return app;
    }

    /// <summary>
    /// Configures headers forwarding options for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The WebApplication instance.</returns>
    public static IApplicationBuilder AddForwardHeaders(this WebApplication app)
    {
        var forwardingOptions = new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.All
        };

        forwardingOptions.KnownNetworks.Clear();
        forwardingOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardingOptions);

        return app;
    }

    /// <summary>
    /// Configures development environment settings for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The WebApplication instance.</returns>
    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }

    /// <summary>
    /// Configures Swagger and Swagger UI for API documentation.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <param name="apiConfiguration">API configuration settings.</param>
    /// <returns>The WebApplication instance.</returns>
    public static IApplicationBuilder UseSwagger(this WebApplication app, ApiConfiguration apiConfiguration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"{apiConfiguration.ApiBaseUrl}/swagger/v1/swagger.json", apiConfiguration.ApiName);

            // Configure OAuth settings for Swagger UI
            c.OAuthClientId(apiConfiguration.OidcSwaggerUIClientId);
            c.OAuthAppName(apiConfiguration.ApiName);
            c.OAuthUsePkce();
        });

        return app;
    }

    /// <summary>
    /// Migrates the database schema to the latest version.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The WebApplication instance.</returns>
    public static IApplicationBuilder MigrateDatabase(this WebApplication app)
    {
        var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceScopeFactory.CreateScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        return app;
    }
}
