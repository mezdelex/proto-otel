# Protobuf + OpenTelemetry + Clean Architecture

<img width="842" height="441" alt="image" src="https://github.com/user-attachments/assets/22f729d1-e93b-4089-86d8-0208c641e5af" />
<img width="1913" height="891" alt="image" src="https://github.com/user-attachments/assets/9655e3ce-2739-4c7f-b3c6-d3139c83332e" />
<img width="1900" height="453" alt="image" src="https://github.com/user-attachments/assets/d16894eb-6db5-4fda-94f3-dc7ae80f3aca" />
<img width="1919" height="1027" alt="image" src="https://github.com/user-attachments/assets/eab831ce-7222-40b1-96c8-edb66f7a1078" />

.NET9 Clean Architecture + DDD + CQRS + Specifications + AutoMapper + Domain Events + Testing + Identity + Redis + Protocol Buffers + OpenTelemetry + OpenSearch

## Docker

`docker-compose up` @ project root

## Migrations

`dotnet ef migrations add <migration_name> --project .\src\Services\Expenses\Infrastructure\Infrastructure.csproj --startup-project .\src\Services\Expenses\WebApi\WebApi.csproj`

## Database

`dotnet ef database update --project .\src\Services\Expenses\Infrastructure\Infrastructure.csproj --startup-project .\src\Services\Expenses\WebApi\WebApi.csproj` or just let `ApplyMigrations` extension migrate automatically on `run/watch`.

<sub>There's stuff that should've been ignored kept public on purpose, like docker-compose's `.env` variables, for discoverability purposes.</sub>
