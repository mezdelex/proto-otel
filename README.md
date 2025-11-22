# Proto OpenTelemetry Template

.NET9 Clean Architecture + DDD + CQRS + Specifications + AutoMapper + Domain Events + Testing + Identity + Redis + Protocol Buffers + OpenTelemetry + OpenSearch

## Docker

`docker-compose up` @ project root

## Migrations

`dotnet ef migrations add <migration_name> --project .\src\Services\Expenses\Infrastructure\Infrastructure.csproj --startup-project .\src\Services\Expenses\WebApi\WebApi.csproj`

## Database

`dotnet ef database update --project .\src\Services\Expenses\Infrastructure\Infrastructure.csproj --startup-project .\src\Services\Expenses\WebApi\WebApi.csproj` or just let `ApplyMigrations` extension migrate automatically on `run/watch`.

<sub>There's stuff that should've been ignored kept public on purpose, like docker-compose's `.env` variables, for discoverability purposes.</sub>
