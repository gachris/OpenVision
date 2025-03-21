using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var databaseProvider = builder.Configuration.GetSection("Parameters").GetValue<string>("databaseProvider")!;

IResourceBuilder<IResourceWithConnectionString> resourceBuilder;

if (databaseProvider == "SqlServer")
{
    resourceBuilder = builder.AddSqlServer("sqlserver")
                             .WithDataVolume()
                             .AddDatabase("vision");

}
else if (databaseProvider == "MySql")
{
    resourceBuilder = builder.AddMySql("mysql")
                             .WithDataVolume()
                             .AddDatabase("vision");
}
else
{
    resourceBuilder = builder.AddPostgres("postgres")
                             .WithDataVolume()
                             .AddDatabase("vision");
}

var databaseProviderParameter = builder.AddParameter("databaseProvider");

builder.AddProject<Projects.OpenVision_Server>("server")
       .WithEnvironment("DatabaseProvider", databaseProviderParameter)
       .WithReference(resourceBuilder);

builder.AddProject<Projects.OpenVision_Client>("client");

builder.Build().Run();