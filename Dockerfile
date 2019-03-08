FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /sln

# Copy sln and all the csproj files and restore to cache the layer for faster builds.
COPY ./MessengerApi.sln ./
COPY ./Abstractions/Abstractions.csproj ./Abstractions/Abstractions.csproj
COPY ./DataAccess.Mongo/DataAccess.Mongo.csproj ./DataAccess.Mongo/DataAccess.Mongo.csproj
COPY ./DataAccess.Mongo.Tests/DataAccess.Mongo.Tests.csproj ./DataAccess.Mongo.Tests/DataAccess.Mongo.Tests.csproj
COPY ./DataAccess.Redis/DataAccess.Redis.csproj ./DataAccess.Redis/DataAccess.Redis.csproj
COPY ./DataAccess.Redis.Tests/DataAccess.Redis.Tests.csproj ./DataAccess.Redis.Tests/DataAccess.Redis.Tests.csproj
COPY ./WebApi/WebApi.csproj ./WebApi/WebApi.csproj

RUN dotnet restore

# Copy everything else and build
COPY ./Abstractions ./Abstractions
COPY ./DataAccess.Mongo ./DataAccess.Mongo
COPY ./DataAccess.Mongo.Tests ./DataAccess.Mongo.Tests
COPY ./DataAccess.Redis ./DataAccess.Redis
COPY ./DataAccess.Redis.Tests ./DataAccess.Redis.Tests
COPY ./WebApi ./WebApi

RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /sln/WebApi/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]