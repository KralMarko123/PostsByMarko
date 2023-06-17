#Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7171

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "PostsByMarko.Host/PostsByMarko.Host.csproj" .
RUN dotnet restore "PostsByMarko.Host.csproj"

COPY "PostsByMarko.Host/" .
RUN dotnet build "PostsByMarko.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostsByMarko.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostsByMarko.Host.dll"]