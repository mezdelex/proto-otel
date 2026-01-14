FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY ./protos/ ./protos/
COPY ./src/Services/Notifications/ ./src/Services/Notifications/

RUN dotnet restore ./src/Services/Notifications/Notifications.csproj
RUN dotnet build ./src/Services/Notifications/Notifications.csproj -c Release
RUN dotnet publish ./src/Services/Notifications/Notifications.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8002 8102

ENTRYPOINT ["dotnet", "Notifications.dll"]
