# Base image cho build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution vào container
COPY ../../CineBook_Backend.sln ./
COPY ../../PricingService ./PricingService

# Restore và build
WORKDIR /src/PricingService/PricingService.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PricingService.API.dll"]
