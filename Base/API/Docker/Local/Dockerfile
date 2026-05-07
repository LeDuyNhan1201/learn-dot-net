FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

EXPOSE 60000

ENV DOTNET_USE_POLLING_FILE_WATCHER=1

ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://0.0.0.0:60000"]