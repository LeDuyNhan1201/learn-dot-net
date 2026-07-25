```shell
dotnet tool install --global dotnet-ef

~/.dotnet/tools/dotnet-ef database drop \
    --project Modules/Restaurant/Restaurant.Infrastructure \
    --startup-project Modules/Restaurant/Restaurant.API \
    -- --environment Local

~/.dotnet/tools/dotnet-ef migrations remove \
    --project Modules/Restaurant/Restaurant.Infrastructure \
    --startup-project Modules/Restaurant/Restaurant.API \
    -- --environment Local

# Run more than once if you have multiple migrations to remove
~/.dotnet/tools/dotnet-ef migrations remove \
    --project Modules/Restaurant/Restaurant.Infrastructure \
    --startup-project Modules/Restaurant/Restaurant.API \
    -- --environment Local

~/.dotnet/tools/dotnet-ef migrations add InitialCreate \
    --project Modules/Restaurant/Restaurant.Infrastructure \
    --startup-project Modules/Restaurant/Restaurant.API \
    -o Persistence/Migrations \
    -- --environment Local

~/.dotnet/tools/dotnet-ef database update \
    --project Modules/Restaurant/Restaurant.Infrastructure \
    --startup-project Modules/Restaurant/Restaurant.API \
    -- --environment Local
```

