# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/sdk/tags
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build

ARG BUILD_CONFIGURATION=Release
ARG RUNTIME_IDENTIFIER=linux-musl-x64

WORKDIR /src

# Copy solution + csproj files first for cache optimization
COPY --link Backend.slnx ./

COPY --link BuildingBlocks/BuildingBlocks.API/*.csproj BuildingBlocks/BuildingBlocks.API/
COPY --link BuildingBlocks/BuildingBlocks.Application/*.csproj BuildingBlocks/BuildingBlocks.Application/
COPY --link BuildingBlocks/BuildingBlocks.Domain/*.csproj BuildingBlocks/BuildingBlocks.Domain/
COPY --link BuildingBlocks/BuildingBlocks.Infrastructure/*.csproj BuildingBlocks/BuildingBlocks.Infrastructure/
COPY --link BuildingBlocks/BuildingBlocks.Shared/*.csproj BuildingBlocks/BuildingBlocks.Shared/

COPY --link Restaurant/Restaurant.API/*.csproj Restaurant/Restaurant.API/
COPY --link Restaurant/Restaurant.Application/*.csproj Restaurant/Restaurant.Application/
COPY --link Restaurant/Restaurant.Domain/*.csproj Restaurant/Restaurant.Domain/
COPY --link Restaurant/Restaurant.Infrastructure/*.csproj Restaurant/Restaurant.Infrastructure/

# Restore
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore Restaurant/Restaurant.API/Restaurant.API.csproj -r "$RUNTIME_IDENTIFIER"

# Copy full source
COPY --link . .

# Publish Native AOT
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/Restaurant/Restaurant.API/bin \
    dotnet publish Restaurant/Restaurant.API/Restaurant.API.csproj \
        -c "$BUILD_CONFIGURATION" \
        -r "$RUNTIME_IDENTIFIER" \
        -p:PublishAot=true \
        -p:UseAppHost=true \
        -p:InvariantGlobalization=false \
        -p:SuppressTrimAnalysisWarnings=false \
        -v diag \
        -o /app/publish \
    && rm -f /app/publish/*.dbg

# Runtime image
# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/runtime-deps/tags
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine

ARG BACKEND_TAG=latest

WORKDIR /app

LABEL org.opencontainers.image.version="${BACKEND_TAG}"

# Install required packages
RUN apk add --no-cache \
    icu-libs \
    ca-certificates

# Copy self-sign root CA
COPY rootCA.crt /usr/local/share/ca-certificates/rootCA.crt

# Update trust store
RUN update-ca-certificates

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENV ASPNETCORE_URLS=http://+:60001

COPY --link --from=build /app/publish .

EXPOSE 60001

ENTRYPOINT ["./Restaurant.API"]
