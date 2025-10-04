FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["UknfCommunicationPlatform.Api/UknfCommunicationPlatform.Api.csproj", "UknfCommunicationPlatform.Api/"]
COPY ["UknfCommunicationPlatform.Core/UknfCommunicationPlatform.Core.csproj", "UknfCommunicationPlatform.Core/"]
COPY ["UknfCommunicationPlatform.Infrastructure/UknfCommunicationPlatform.Infrastructure.csproj", "UknfCommunicationPlatform.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "UknfCommunicationPlatform.Api/UknfCommunicationPlatform.Api.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/UknfCommunicationPlatform.Api"
RUN dotnet publish "UknfCommunicationPlatform.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UknfCommunicationPlatform.Api.dll"]
