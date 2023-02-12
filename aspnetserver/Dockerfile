#Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "aspnetserver/aspnetserver.csproj" .
RUN dotnet restore "aspnetserver.csproj"

COPY "aspnetserver/" .
RUN dotnet build "aspnetserver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "aspnetserver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "aspnetserver.dll"]