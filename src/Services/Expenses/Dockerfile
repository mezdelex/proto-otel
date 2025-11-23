FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ./protos/ ./protos/
COPY ./src/Services/Expenses/ ./src/Services/Expenses/

RUN dotnet restore ./src/Services/Expenses/WebApi/WebApi.csproj
RUN dotnet build ./src/Services/Expenses/WebApi/WebApi.csproj -c Release
RUN dotnet publish ./src/Services/Expenses/WebApi/WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8001 8101

ENTRYPOINT ["dotnet", "WebApi.dll"]
