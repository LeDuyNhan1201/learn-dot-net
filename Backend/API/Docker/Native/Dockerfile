# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/sdk/tags
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build

ARG BUILD_CONFIGURATION=Release
ARG RUNTIME_IDENTIFIER=linux-musl-x64
ARG BACKEND_TAG=latest

WORKDIR /src

COPY --link ["API.csproj", "./"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "API.csproj" -r "$RUNTIME_IDENTIFIER"

COPY --link . .
WORKDIR /src

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/bin \
    dotnet publish "API.csproj" \
        -c "$BUILD_CONFIGURATION" \
        -r "$RUNTIME_IDENTIFIER" \
        -p:PublishAot=true \
        -p:UseAppHost=true \
        -o /app/publish \
    && rm -f /app/publish/*.dbg /app/publish/*.Development.json

# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/runtime-deps/tags
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine

ARG BACKEND_TAG=latest

WORKDIR /app

LABEL org.opencontainers.image.version="$BACKEND_TAG"

ENV ASPNETCORE_URLS=http://+:60001

COPY --link --from=build /app/publish .

EXPOSE 60001

ENTRYPOINT ["./API"]
