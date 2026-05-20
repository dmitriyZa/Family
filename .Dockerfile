FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем всю корневую папку — так мы захватим и back, и Family.Shared
COPY . .

# Восстанавливаем зависимости и собираем проект
RUN dotnet restore "back/back.csproj"
RUN dotnet build "back/back.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS publish
WORKDIR /app
COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "back.dll"]
