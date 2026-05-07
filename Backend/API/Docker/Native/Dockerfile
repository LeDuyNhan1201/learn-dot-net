# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/sdk/tags
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build

ARG BUILD_CONFIGURATION=Release
ARG RUNTIME_IDENTIFIER=linux-musl-x64

WORKDIR /src

COPY --link ["API/API.csproj", "API/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "API/API.csproj" -r "$RUNTIME_IDENTIFIER"

COPY --link . .
WORKDIR /src/API

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/src/API/bin \
    dotnet publish "API.csproj" \
        -c "$BUILD_CONFIGURATION" \
        -r "$RUNTIME_IDENTIFIER" \
        -p:PublishAot=true \
        -p:UseAppHost=true \
        -o /app/publish \
    && rm -f /app/publish/*.dbg /app/publish/*.Development.json

# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/runtime-deps/tags
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:60000

COPY --link --from=build /app/publish .

EXPOSE 60000

ENTRYPOINT ["./API"]
