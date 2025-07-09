FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

WORKDIR /app/BlogFlow.API
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/BlogFlow.API/out .

ENTRYPOINT ["dotnet", "BlogFlow.API.dll"]
