FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY ["src/OpenVision.IdentityServer.Admin/OpenVision.IdentityServer.Admin.csproj", "src/OpenVision.IdentityServer.Admin/"]
COPY ["src/OpenVision.IdentityServer.Admin.EntityFramework.Shared/OpenVision.IdentityServer.Admin.EntityFramework.Shared.csproj", "src/OpenVision.IdentityServer.Admin.EntityFramework.Shared/"]
COPY ["src/OpenVision.IdentityServer.Admin.EntityFramework.SqlServer/OpenVision.IdentityServer.Admin.EntityFramework.SqlServer.csproj", "src/OpenVision.IdentityServer.Admin.EntityFramework.SqlServer/"]
COPY ["src/OpenVision.IdentityServer.Shared/OpenVision.IdentityServer.Shared.csproj", "src/OpenVision.IdentityServer.Shared/"]
COPY ["src/OpenVision.IdentityServer.Admin.EntityFramework.PostgreSQL/OpenVision.IdentityServer.Admin.EntityFramework.PostgreSQL.csproj", "src/OpenVision.IdentityServer.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/OpenVision.IdentityServer.Admin.EntityFramework.MySql/OpenVision.IdentityServer.Admin.EntityFramework.MySql.csproj", "src/OpenVision.IdentityServer.Admin.EntityFramework.MySql/"]
COPY ["src/OpenVision.ServiceDefaults/OpenVision.ServiceDefaults.csproj", "src/OpenVision.ServiceDefaults/"]
RUN dotnet restore -a $TARGETARCH "src/OpenVision.IdentityServer.Admin/OpenVision.IdentityServer.Admin.csproj"
COPY . .
WORKDIR "/src/src/OpenVision.IdentityServer.Admin"
RUN dotnet build -a $TARGETARCH "OpenVision.IdentityServer.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "OpenVision.IdentityServer.Admin.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "OpenVision.IdentityServer.Admin.dll"]