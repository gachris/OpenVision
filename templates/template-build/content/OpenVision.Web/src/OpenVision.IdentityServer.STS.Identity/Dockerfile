FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["src/OpenVision.IdentityServer.STS.Identity/OpenVision.IdentityServer.STS.Identity.csproj", "src/OpenVision.IdentityServer.STS.Identity/"]
COPY ["src/OpenVision.IdentityServer.Admin.EntityFramework.Shared/OpenVision.IdentityServer.Admin.EntityFramework.Shared.csproj", "src/OpenVision.IdentityServer.Admin.EntityFramework.Shared/"]
COPY ["src/OpenVision.IdentityServer.Shared/OpenVision.IdentityServer.Shared.csproj", "src/OpenVision.IdentityServer.Shared/"]
COPY ["src/OpenVision.ServiceDefaults/OpenVision.ServiceDefaults.csproj", "src/OpenVision.ServiceDefaults/"]
RUN dotnet restore -a $TARGETARCH "src/OpenVision.IdentityServer.STS.Identity/OpenVision.IdentityServer.STS.Identity.csproj"
COPY . .
WORKDIR "/src/src/OpenVision.IdentityServer.STS.Identity"
RUN dotnet build -a $TARGETARCH "OpenVision.IdentityServer.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "OpenVision.IdentityServer.STS.Identity.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "OpenVision.IdentityServer.STS.Identity.dll"]