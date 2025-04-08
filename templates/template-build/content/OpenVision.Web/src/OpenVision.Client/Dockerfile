FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/OpenVision.Client/OpenVision.Client.csproj", "src/OpenVision.Client/"]
COPY ["src/OpenVision.ServiceDefaults/OpenVision.ServiceDefaults.csproj", "src/OpenVision.ServiceDefaults/"]

RUN dotnet restore "src/OpenVision.Client/OpenVision.Client.csproj"
COPY . .
WORKDIR "/src/src/OpenVision.Client"
RUN dotnet build "OpenVision.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenVision.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "OpenVision.Client.dll"]