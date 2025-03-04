# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

# Switch to root for installation
USER root

# SkiaSharp dependencies
RUN apt-get update && apt-get install -y \
    sudo \
	libfontconfig1 \
    libfreetype6 \
    libpng16-16 \
    libjpeg62-turbo \
    libwebp7 \
    libtiff6 \
    libgif7 \
    libharfbuzz0b \
    libicu-dev \
    --no-install-recommends

#Switch to non-root user
USER $APP_UID

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SecureImageTransmissionAPI/SecureImageTransmissionAPI.csproj", "SecureImageTransmissionAPI/"]
RUN dotnet restore "./SecureImageTransmissionAPI/SecureImageTransmissionAPI.csproj"
COPY . .
WORKDIR "/src/SecureImageTransmissionAPI"
RUN dotnet build "./SecureImageTransmissionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SecureImageTransmissionAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecureImageTransmissionAPI.dll"]