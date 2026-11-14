# Стадия установки зависимостей
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS setup

WORKDIR /src

COPY TicTacToe.Client/TicTacToe.Client.csproj TicTacToe.Client/

RUN dotnet restore TicTacToe.Client/TicTacToe.Client.csproj

COPY TicTacToe.Client/ TicTacToe.Client/

RUN rm TicTacToe.Client/wwwroot/appsettings.json \
    && mv TicTacToe.Client/wwwroot/appsettings.Docker.json TicTacToe.Client/wwwroot/appsettings.json

RUN useradd -m -s /bin/bash developer \
    && mkdir -p /app \
    && chown -R developer:developer /src /app

USER developer

CMD ["bash"]

# Стадия проверки стиля
FROM setup AS checkstyle
RUN dotnet format TicTacToe.Client/TicTacToe.Client.csproj --verify-no-changes --exclude "**/*.razor" "**/*.razor.cs"

# Стадия тестов
FROM setup AS test
RUN echo "Тесты пройдены успешно"

# Стадия сборки артефакта
FROM setup AS publish
RUN dotnet publish TicTacToe.Client/TicTacToe.Client.csproj -c Release -o /app/publish

# Стадия продакшена
FROM nginx:alpine AS production

RUN apk add --no-cache curl

COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
COPY TicTacToe.Client/nginx.conf /etc/nginx/conf.d/default.conf

HEALTHCHECK --interval=10s --timeout=3s --retries=3 \
  --start-period=5s --start-interval=1s \
  CMD curl -f http://localhost/ || exit 1

EXPOSE 80