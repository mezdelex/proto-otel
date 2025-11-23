FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY ./src/Gateway/ ./src/Gateway/

RUN dotnet restore ./src/Gateway/Gateway.csproj
RUN dotnet build ./src/Gateway/Gateway.csproj -c Release
RUN dotnet publish ./src/Gateway/Gateway.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8000 8100

ENTRYPOINT ["dotnet", "Gateway.dll"]
