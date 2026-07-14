# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy solution + project files
COPY --link Backend.slnx ./

COPY --link BuildingBlocks/BuildingBlocks.API/*.csproj BuildingBlocks/BuildingBlocks.API/
COPY --link BuildingBlocks/BuildingBlocks.Application/*.csproj BuildingBlocks/BuildingBlocks.Application/
COPY --link BuildingBlocks/BuildingBlocks.Domain/*.csproj BuildingBlocks/BuildingBlocks.Domain/
COPY --link BuildingBlocks/BuildingBlocks.Infrastructure/*.csproj BuildingBlocks/BuildingBlocks.Infrastructure/
COPY --link BuildingBlocks/BuildingBlocks.SharedKernel/*.csproj BuildingBlocks/BuildingBlocks.SharedKernel/

COPY --link Modules/Restaurant/Restaurant.API/*.csproj Modules/Restaurant/Restaurant.API/
COPY --link Modules/Restaurant/Restaurant.Application/*.csproj Modules/Restaurant/Restaurant.Application/
COPY --link Modules/Restaurant/Restaurant.Domain/*.csproj Modules/Restaurant/Restaurant.Domain/
COPY --link Modules/Restaurant/Restaurant.Infrastructure/*.csproj Modules/Restaurant/Restaurant.Infrastructure/

# Restore
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore Modules/Restaurant/Restaurant.API/Restaurant.API.csproj

# Copy source
COPY --link . .

# Publish
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/Modules/Restaurant/Restaurant.API/bin \
    dotnet publish Modules/Restaurant/Restaurant.API/Restaurant.API.csproj \
        -c "$BUILD_CONFIGURATION" \
        --no-restore \
        -o /app/publish

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

ARG BACKEND_TAG=latest

WORKDIR /app

LABEL org.opencontainers.image.version="${BACKEND_TAG}"

# Install required packages
RUN apk add --no-cache \
    openssl \
    icu-libs \
    ca-certificates \
    krb5-libs

# Trust custom CA
COPY rootCA.crt /usr/local/share/ca-certificates/rootCA.crt

RUN update-ca-certificates

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:60001

COPY --link --from=build /app/publish .

EXPOSE 60001

ENTRYPOINT ["dotnet", "Restaurant.API.dll"]