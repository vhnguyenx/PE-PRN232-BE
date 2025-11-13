# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["BE-PE/BE-PE.csproj", "BE-PE/"]
RUN dotnet restore "BE-PE/BE-PE.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/BE-PE"
RUN dotnet build "BE-PE.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BE-PE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BE-PE.dll"]
