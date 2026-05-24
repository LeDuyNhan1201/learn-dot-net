# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/sdk/tags
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build

ARG BUILD_CONFIGURATION=Release
ARG RUNTIME_IDENTIFIER=linux-musl-x64

WORKDIR /src

# Copy solution + csproj files first for cache optimization
COPY --link Backend.slnx ./

COPY --link BuildingBlocks.API/*.csproj BuildingBlocks.API/
COPY --link BuildingBlocks.Application/*.csproj BuildingBlocks.Application/
COPY --link BuildingBlocks.Domain/*.csproj BuildingBlocks.Domain/
COPY --link BuildingBlocks.Infrastructure/*.csproj BuildingBlocks.Infrastructure/

COPY --link API/*.csproj API/
COPY --link Application/*.csproj Application/
COPY --link Domain/*.csproj Domain/
COPY --link Infrastructure/*.csproj Infrastructure/

# Restore
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore API/API.csproj -r "$RUNTIME_IDENTIFIER"

# Copy full source
COPY --link . .

# Publish Native AOT
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/API/bin \
    dotnet publish API/API.csproj \
        -c "$BUILD_CONFIGURATION" \
        -r "$RUNTIME_IDENTIFIER" \
        -p:PublishAot=true \
        -p:UseAppHost=true \
        -p:InvariantGlobalization=false \
        -o /app/publish \
    && rm -f /app/publish/*.dbg

# Runtime image
# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/runtime-deps/tags
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine

ARG BACKEND_TAG=latest

WORKDIR /app

LABEL org.opencontainers.image.version="${BACKEND_TAG}"

# Required for localization/globalization
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENV ASPNETCORE_URLS=http://+:60001

COPY --link --from=build /app/publish .

EXPOSE 60001

ENTRYPOINT ["./API"]
