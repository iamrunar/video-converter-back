# https://docs.docker.com/language/dotnet/build-images/
# https://github.com/dotnet/dotnet-docker/tree/main
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-7.0
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/*.sln ./
COPY src/gateway/*.csproj ./gateway/
RUN dotnet restore --packages .nuget

# copy everything else and build app
COPY src/gateway/. ./gateway/
WORKDIR /source/gateway
RUN dotnet publish -c Release -o /app/ --packages .nuget --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "gateway.dll"]