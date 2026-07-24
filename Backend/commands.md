```shell
dotnet ef migrations add InitialCreate \
    --project Restaurant.Infrastructure \
    --startup-project Restaurant.API \
    -o Persistence/Migrations
```

