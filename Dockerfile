FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY src/MeDotNet/*.csproj src/MeDotNet/
RUN dotnet restore src/MeDotNet/MeDotNet.csproj

COPY src/ src/
RUN dotnet publish src/MeDotNet/MeDotNet.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MeDotNet.dll"]
