using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotChocolate.Execution.Configuration;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Configuration;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Filters;
using OpenVision.Server.Core.GraphQL;
using OpenVision.Server.Core.Helpers;
using OpenVision.Server.Core.Mappers;
using OpenVision.Server.Core.Repositories;
using OpenVision.Server.Core.Services;
using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Contains extension methods for configuring the OpenVision API application.
/// This class centralizes the registration and configuration of common services, middleware, 
/// authentication, authorization, database contexts, GraphQL, Swagger/OpenAPI, CORS, and other 
/// application-specific features. Using these extension methods ensures that the application is 
/// consistently configured across different environments.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers MediatR services and pipeline behaviors with the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the MediatR services will be added.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddDefaultMediatR(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR((configuration) => configuration.RegisterServicesFromAssembly(typeof(Extensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds GraphQL server support with filtering, sorting, authorization, and cost options.
    /// Optionally registers a pooled DbContext factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="usePooledDbContext">If true, registers a pooled DbContext factory.</param>
    /// <returns>The updated request executor builder.</returns>
    public static IRequestExecutorBuilder AddGraphQL(this IServiceCollection services, bool usePooledDbContext)
    {
        ArgumentNullException.ThrowIfNull(services);

        var requestExecutorBuilder = services.AddGraphQLServer();
        requestExecutorBuilder.AddAuthorization();
        requestExecutorBuilder.InitializeOnStartup();
        requestExecutorBuilder.AddProjections();
        requestExecutorBuilder.AddFiltering();
        requestExecutorBuilder.AddSorting();
        requestExecutorBuilder.AddQueryType<Query>();
        requestExecutorBuilder.AddMutationType<Mutation>();
        requestExecutorBuilder.ModifyCostOptions(options =>
        {
            options.EnforceCostLimits = false;
            options.ApplyCostDefaults = false;
        });

        if (usePooledDbContext)
        {
            requestExecutorBuilder.RegisterDbContextFactory<ApplicationDbContext>();
        }

        return requestExecutorBuilder;
    }

    /// <summary>
    /// Registers AutoMapper and scans for profiles in the assembly containing <see cref="MappingProfile"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }

    /// <summary>
    /// Registers repositories for dependency injection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<IDatabasesRepository, DatabasesRepository>();
        services.AddTransient<IApiKeysRepository, ApiKeysRepository>();
        services.AddTransient<IImageTargetsRepository, ImageTargetsRepository>();
        return services;
    }

    /// <summary>
    /// Registers application-level transient services for dependency injection.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <returns>The IServiceCollection instance with application services registered.</returns>
    public static IServiceCollection AddOpenVisionCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<ITrackablesService, TrackablesService>();
        services.AddTransient<IDatabasesService, DatabasesService>();
        services.AddTransient<IFilesService, FilesService>();
        services.AddTransient<ITargetsService, TargetsService>();
        services.AddTransient<IApiKeyGeneratorService, ApiKeyGeneratorService>();
        return services;
    }

    /// <summary>
    /// Registers the current user service for dependency injection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        return services;
    }

    /// <summary>
    /// Configures the application's DbContext using the provided connection string and database configuration.
    /// Optionally accepts additional DbContext options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="databaseProviderConfiguration">The database provider configuration.</param>
    /// <param name="optionsAction">Optional action for additional DbContext configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        string connectionString,
        DatabaseConfiguration databaseProviderConfiguration,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentNullException.ThrowIfNull(databaseProviderConfiguration);

        services.AddOpenVisionDbContext(options =>
        {
            options.ConfigureDbContextOptionsBuilder(connectionString, databaseProviderConfiguration);
            optionsAction?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Configures and registers a pooled DbContext factory using the provided connection string and database configuration.
    /// Optionally accepts additional DbContext options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="databaseProviderConfiguration">The database provider configuration.</param>
    /// <param name="optionsAction">Optional action for additional DbContext configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPooledDbContextFactory(
        this IServiceCollection services,
        string connectionString,
        DatabaseConfiguration databaseProviderConfiguration,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentNullException.ThrowIfNull(databaseProviderConfiguration);

        services.AddOpenVisionPooledDbContextFactory(options =>
        {
            options.ConfigureDbContextOptionsBuilder(connectionString, databaseProviderConfiguration);
            optionsAction?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Configures the DbContextOptionsBuilder for a given connection string and database provider.
    /// </summary>
    /// <param name="options">The DbContextOptionsBuilder to configure.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="databaseProviderConfiguration">The database provider configuration.</param>
    /// <returns>The configured DbContextOptionsBuilder.</returns>
    public static DbContextOptionsBuilder ConfigureDbContextOptionsBuilder(
        this DbContextOptionsBuilder options,
        string connectionString,
        DatabaseConfiguration databaseProviderConfiguration)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentNullException.ThrowIfNull(databaseProviderConfiguration);

        var migrationAssembly = MigrationAssemblyHelper.GetMigrationAssemblyByProvider(databaseProviderConfiguration);

        switch (databaseProviderConfiguration.ProviderType)
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
                throw new ArgumentOutOfRangeException(nameof(databaseProviderConfiguration.ProviderType), "Unsupported database provider type.");
        }

        return options;
    }

    /// <summary>
    /// Adds default JWT authentication configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>An AuthenticationBuilder for further configuration.</returns>
    public static AuthenticationBuilder AddDefaultAuthentication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        return authenticationBuilder;
    }

    /// <summary>
    /// Adds JWT Bearer authentication using the provided OIDC configuration.
    /// </summary>
    /// <param name="authenticationBuilder">The AuthenticationBuilder instance.</param>
    /// <param name="oidcConfiguration">The OIDC configuration.</param>
    /// <returns>The updated AuthenticationBuilder.</returns>
    public static AuthenticationBuilder AddDefaultJwtBearer(this AuthenticationBuilder authenticationBuilder, OidcConfiguration oidcConfiguration)
    {
        ArgumentNullException.ThrowIfNull(authenticationBuilder);
        ArgumentNullException.ThrowIfNull(oidcConfiguration);

        authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = oidcConfiguration.Authority;
            options.RequireHttpsMetadata = oidcConfiguration.RequireHttpsMetadata;
            options.Audience = oidcConfiguration.Audience;
        });

        return authenticationBuilder;
    }

    public static AuthenticationBuilder AddApiKeyScheme(this AuthenticationBuilder authenticationBuilder)
    {
        ArgumentNullException.ThrowIfNull(authenticationBuilder);
        authenticationBuilder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, null);
        return authenticationBuilder;
    }

    /// <summary>
    /// Adds a default authorization policy that requires authentication using the Bearer scheme and specified scopes.
    /// </summary>
    /// <param name="authorizationBuilder">The AuthorizationBuilder instance.</param>
    /// <param name="oidcConfiguration">The OIDC configuration.</param>
    /// <returns>The updated AuthorizationBuilder.</returns>
    public static AuthorizationBuilder AddDefaultPolicy(this AuthorizationBuilder authorizationBuilder, OidcConfiguration oidcConfiguration)
    {
        ArgumentNullException.ThrowIfNull(authorizationBuilder);
        ArgumentNullException.ThrowIfNull(oidcConfiguration);

        authorizationBuilder.AddPolicy(AuthorizationConsts.BearerPolicy, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireScope();
            foreach (var scope in oidcConfiguration.Scopes)
            {
                policy.RequireClaim("scope", scope);
            }
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        });

        authorizationBuilder.AddPolicy(AuthorizationConsts.ClientApiKeyPolicy, policy =>
        {
            policy.RequireAssertion(context =>
            {
                return context.User.HasClaim(c => c.Type == ApiKeyDefaults.X_API_KEY)
                       && context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Client");
            })
            .AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme);
        });

        authorizationBuilder.AddPolicy(AuthorizationConsts.ServerApiKeyPolicy, policy =>
        {
            policy.RequireAssertion(context =>
            {
                return context.User.HasClaim(c => c.Type == ApiKeyDefaults.X_API_KEY)
                       && context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Server");
            })
            .AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme);
        });

        return authorizationBuilder;
    }

    /// <summary>
    /// Adds a default CORS policy using the specified CORS configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="corsConfiguration">The CORS configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDefaultCors(this IServiceCollection services, CorsConfiguration corsConfiguration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(corsConfiguration);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    if (corsConfiguration.CorsAllowAnyOrigin)
                    {
                        builder.AllowAnyOrigin();
                    }
                    else if (corsConfiguration.CorsAllowOrigins != null && corsConfiguration.CorsAllowOrigins.Length > 0)
                    {
                        builder.WithOrigins(corsConfiguration.CorsAllowOrigins);
                    }

                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });

        return services;
    }

    /// <summary>
    /// Adds controllers with default filters including model validation and authorization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>An IMvcBuilder for further configuration.</returns>
    public static IMvcBuilder AddDefaultControllers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var mvcBuilder = services.AddControllers(options =>
        {
            options.Filters.Add(new ValidateModelFilter());
        });

        return mvcBuilder;
    }

    /// <summary>
    /// Configures default JSON serialization options for controllers.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder.</param>
    /// <returns>The updated IMvcBuilder.</returns>
    public static IMvcBuilder AddDefaultJsonOptions(this IMvcBuilder mvcBuilder)
    {
        ArgumentNullException.ThrowIfNull(mvcBuilder);

        mvcBuilder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        return mvcBuilder;
    }

    /// <summary>
    /// Adds and configures Swagger generation with OAuth2 security definitions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="swaggerConfiguration">The Swagger configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDefaultSwaggerGen(this IServiceCollection services, SwaggerConfiguration swaggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(swaggerConfiguration);

        services.AddSingleton(swaggerConfiguration);

        services.AddSwaggerGen(options =>
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = swaggerConfiguration.SwaggerName,
                Version = swaggerConfiguration.Version
            };

            options.SwaggerDoc(swaggerConfiguration.Version, openApiInfo);

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerConfiguration.AuthorizationUrl),
                        TokenUrl = new Uri(swaggerConfiguration.TokenUrl),
                        Scopes = swaggerConfiguration.Scopes.ToDictionary(x => x, y => y)
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    /// <summary>
    /// Configures forwarded headers to ensure proper handling of proxy headers.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseDefaultsForwardHeaders(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var forwardingOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        };

        forwardingOptions.KnownNetworks.Clear();
        forwardingOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardingOptions);
        return app;
    }

    /// <summary>
    /// Configures a global exception handler that returns JSON-formatted error responses.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder ConfigureExceptionHandler(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        const string contentType = "application/json";

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var errorMessage = app.Environment.IsDevelopment()
                        ? contextFeature.Error.Message
                        : "An unexpected error occurred. Please try again later.";

                    var errorCollection = new List<OpenVision.Shared.Responses.Error>
                    {
                        new (ResultCode.InternalServerError, errorMessage)
                    };

                    var response = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);
                    var result = JsonSerializer.Serialize(response, jsonSerializerOptions);

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = contentType;
                    await context.Response.WriteAsync(result);
                }
            });
        });
        return app;
    }

    /// <summary>
    /// Configures Swagger and Swagger UI using the specified configuration action.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>  
    /// <param name="swaggerConfiguration">The Swagger configuration settings.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, SwaggerConfiguration swaggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(swaggerConfiguration);

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(swaggerConfiguration.SwaggerEndpoint, swaggerConfiguration.SwaggerName);
            c.OAuthClientId(swaggerConfiguration.OAuthClientId);
            c.OAuthAppName(swaggerConfiguration.OAuthAppName);
            c.OAuthUsePkce();
        });

        return app;
    }

    /// <summary>
    /// Applies pending migrations to the database.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <param name="usePooledDbContext">Indicates whether a pooled DbContext factory should be used.</param>
    /// <returns>The updated <see cref="WebApplication"/> instance.</returns>
    public static IApplicationBuilder MigrateDatabase(this WebApplication app, bool usePooledDbContext)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var serviceScope = app.Services.CreateScope();

        if (usePooledDbContext)
        {
            var dbContextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var context = dbContextFactory.CreateDbContext();
            context.Database.Migrate();
        }
        else
        {
            using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        return app;
    }
}
