FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

EXPOSE 60000
EXPOSE 60001

ENV DOTNET_USE_POLLING_FILE_WATCHER=1

# Create dev certificate
RUN mkdir -p /https \
    && chmod 777 /https \
    && dotnet dev-certs https -ep /https/cert.pfx -p password

# set env for Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

ENTRYPOINT ["dotnet", "watch", "run", "--urls=https://0.0.0.0:60001;http://0.0.0.0:60000"]