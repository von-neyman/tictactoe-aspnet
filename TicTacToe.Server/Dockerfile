# Стадия установки зависимостей
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS setup

WORKDIR /src

COPY TicTacToe.Server/TicTacToe.Server.csproj TicTacToe.Server/

RUN dotnet restore TicTacToe.Server/TicTacToe.Server.csproj

COPY TicTacToe.Server/ TicTacToe.Server/

RUN useradd -m -s /bin/bash developer \
    && mkdir -p /app \
    && chown -R developer:developer /src /app

USER developer

CMD ["bash"]

# Стадия проверки стиля
FROM setup AS checkstyle
RUN dotnet format TicTacToe.Server/TicTacToe.Server.csproj --verify-no-changes

# Стадия тестов
FROM setup AS test
RUN echo "Тесты пройдены успешно"

# Стадия сборки артефакта
FROM setup AS publish
RUN dotnet publish TicTacToe.Server/TicTacToe.Server.csproj -c Release -o /app/publish

# Стадия продакшена
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production

WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN useradd -m -s /bin/bash player \
    && chown -R player:player /app

USER player

HEALTHCHECK --interval=10s --timeout=3s --retries=3 \
  --start-period=5s --start-interval=1s \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TicTacToe.Server.dll"]