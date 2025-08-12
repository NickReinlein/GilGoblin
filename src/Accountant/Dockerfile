FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Accountant.csproj", "./"]
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 55452

ENTRYPOINT ["dotnet", "Accountant.dll"]