#Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
ARG CSPROJ_FILE_PATH="src/PostsByMarko.Host/PostsByMarko.Host.csproj"
WORKDIR /app

COPY . .

RUN dotnet restore $CSPROJ_FILE_PATH

RUN dotnet publish $CSPROJ_FILE_PATH  --output out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

EXPOSE 7171

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PostsByMarko.Host.dll"]

