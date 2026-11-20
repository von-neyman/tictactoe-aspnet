# TicTacToe

[![CI/CD](https://github.com/von-neyman/tictactoe-aspnet/actions/workflows/tictactoe-aspnet.yml/badge.svg)](https://github.com/von-neyman/tictactoe-aspnet/actions/workflows/tictactoe-aspnet.yml)

Игра «Крестики-Нолики» с компьютером. Сервер на ASP.NET Core Web API, клиент на Blazor WebAssembly. Проект создан в учебных целях в рамках обучения в Школе 21.

![Игровой процесс](screenshots/game.png)

## Быстрый старт

```bash
git clone https://github.com/von-neyman/tictactoe-aspnet.git
cd tictactoe-aspnet
```

### Visual Studio

1. Открыть `TicTacToe.slnx`
2. Настроить несколько запускаемых проектов: `TicTacToe.Server` и `TicTacToe.Client` (правой кнопкой по решению → Настроить запускаемые проекты → Несколько запускаемых проектов → Запуск для обоих)
3. Запустить (F5)

### Visual Studio Code / терминал

Запустить сервер и клиент в разных терминалах:

```bash
dotnet run --project TicTacToe.Server
dotnet run --project TicTacToe.Client
```

При запуске в консоли выводятся порты, например:

```
Now listening on: http://localhost:5114   ← сервер
Now listening on: http://localhost:5122   ← клиент
```

Эти порты могут отличаться при каждом запуске. Нужно указать их в настройках:

В TicTacToe.Client/wwwroot/appsettings.json указать адрес сервера (тот, что вывелся в консоли сервера):

```json
{
    "ServerUrl": "http://localhost:5114"
}
```

В TicTacToe.Server/appsettings.Development.json указать адрес клиента в CorsOrigins (тот, что вывелся в консоли клиента):

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "CorsOrigins": "http://localhost:5122"
}
```

Перезапустить оба проекта. Открыть в браузере адрес клиента (например, http://localhost:5122).

При запуске через Visual Studio порты фиксированные (7175 и 7160) и уже прописаны в appsettings.json и launchSettings.json. При запуске через терминал порты назначаются динамически — их нужно обновлять в настройках вручную.

### Требования

| Компонент | Примечание |
|-----------|------------|
| .NET SDK | Версия 8.0 или выше |
| Браузер | Любой современный (Chrome, Firefox, Edge) |

![Меню новой игры](screenshots/menu.png)

## Docker

В проекте есть `Dockerfile`, а также `docker-compose.yml`, который связывает контейнеры в одну сеть.

### Архитектура

- `server` — ASP.NET Core Web API. Слушает порт 8080 внутри сети Docker.
- `client` — Blazor WebAssembly, собранный в статику и раздаваемый через nginx. Слушает порт 80 внутри сети.
- `proxy` — внешний nginx. Слушает порт 80 (наружу проброшен на 8080). Это единственная точка входа для браузера.

**proxy** принимает все внешние запросы и маршрутизирует их:

- `/` — отдаёт статику Blazor из `client`.
- `/game/` — проксирует в `server` (API игры).
- `/swagger` — проксирует в `server` (Swagger UI, доступен только в Development).

### Запуск

**Production (локальная сборка):**

```bash
docker compose up --build
```

или в фоне:

```bash
docker compose up -d --build
```

**Production (из GHCR):**

```bash
docker compose -f docker-compose.ghcr.yml up
```

или в фоне:

```bash
docker compose -f docker-compose.ghcr.yml up -d
```

- Открыть http://localhost:8080 в браузере.

**Development (игра + Swagger):**

```bash
# bash
ENV=Development docker compose up --build
```

```powershell
# powershell
$env:ENV="Development"; docker compose up --build
```

- Игра: открыть http://localhost:8080 в браузере.
- Swagger: открыть http://localhost:8080/swagger или http://localhost:8080/swagger/.

### Остановка

```bash
docker compose down
```

или, если запускали из GHCR:

```bash
docker compose -f docker-compose.ghcr.yml down
```

### Сборка отдельных образов

`Dockerfile` (сервер и клиент) используют multi-stage build:

- `setup` — установка зависимостей и копирование кода.
- `checkstyle` — проверка стиля.
- `test` — тесты.
- `publish` — сборка артефакта.
- `production` — финальный образ.

**Сервер:**

```bash
docker build -f TicTacToe.Server/Dockerfile --target production -t tictactoe-server .
```

**Клиент:**

```bash
docker build -f TicTacToe.Client/Dockerfile --target production -t tictactoe-client .
```

**Прокси:**

```bash
docker build -f nginx-proxy/Dockerfile -t tictactoe-proxy .
```

### Запуск отдельных контейнеров

**Создание сети:**

```bash
docker network create tictactoe-net
```

**Запуск сервера:**

```bash
# Production
docker run -d --name server --network tictactoe-net -e CorsOrigins=http://localhost:8080 tictactoe-server
```

```bash
# Development
docker run -d --name server --network tictactoe-net -e ASPNETCORE_ENVIRONMENT=Development -e CorsOrigins=http://localhost:8080 tictactoe-server
```

**Запуск клиента:**

```bash
docker run -d --name client --network tictactoe-net tictactoe-client
```

**Запуск прокси:**

```bash
docker run -d --name proxy --network tictactoe-net -p 8080:80 tictactoe-proxy
```

- Игра: открыть http://localhost:8080 в браузере.
- Swagger: открыть http://localhost:8080/swagger или http://localhost:8080/swagger/.

**Остановка и удаление:**

```bash
docker stop server client proxy
docker rm server client proxy
docker network rm tictactoe-net
```

## Как играть

- Выбрать символ (крестики или нолики) и сложность
- Если выбраны нолики — компьютер ходит первым
- Кликать по пустой клетке, чтобы сделать ход
- Компьютер отвечает автоматически
- Игра заканчивается победой, поражением или ничьей
- Счётчик вверху считает результаты за сессию

## Сложность

| Уровень | Значение | Описание |
|---------|----------|----------|
| Балбес | 0 | Случайные ходы |
| Неосторожный | 1 | Видит только свои ближайшие ходы |
| Опытный | 2 | Видит свои ближайшие ходы и ближайший ход игрока |
| Мастер | 9 | Идеальная игра, просчитывает всё |

## Алгоритм компьютера

Компьютер использует алгоритм Минимакс с настраиваемой глубиной просчёта (параметр `Difficulty`). На каждом ходе перебираются все возможные варианты, для каждого строится дерево игры до заданной глубины, позиции оцениваются: +1 (победа компьютера), -1 (победа игрока), 0 (ничья). Если несколько ходов одинаково хороши — выбирается случайный, чтобы игра не была предсказуемой.

![Завершение игры](screenshots/gameover.png)

## Архитектура проекта

Многослойная архитектура: Domain, Datasource, Web, DI.

| Слой | Назначение |
|------|------------|
| Domain | Бизнес-логика: модели Game и GameField, интерфейс IGameService |
| Datasource | Хранение игр в памяти (ConcurrentDictionary), репозиторий, мапперы, реализация GameService с Минимаксом |
| Web | REST API контроллер, DTO для обмена с клиентом, мапперы |
| DI | Конфигурация зависимостей (GameStorage как singleton и т.д.) |

Клиент — отдельный проект `TicTacToe.Client` на Blazor WebAssembly. Общается с сервером через HTTP, дублирует DTO-модели (ограничение ТЗ).

## Технологии

- C#, .NET 8
- ASP.NET Core Web API
- Blazor WebAssembly
- Алгоритм Минимакс
- ConcurrentDictionary
- Dependency Injection
- CORS
- Swagger
- Docker, Docker Compose
- CI/CD

## Документация

Подробные инструкции по настройке и запуску:

- [Настройка базы данных PostgreSQL](docs/database-setup.md)