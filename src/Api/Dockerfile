FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ENV ConnectionStrings__GilGoblinDbContext=$ConnectionStrings__GilGoblinDbContext

WORKDIR /app
COPY . .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 80

ENTRYPOINT ["dotnet", "Api.dll"]

