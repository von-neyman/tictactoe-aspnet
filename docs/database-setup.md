# Настройка базы данных PostgreSQL

Настраиваем подключение проекта к PostgreSQL.

## Установка PostgreSQL

Скачиваем и устанавливаем PostgreSQL. Настройки по умолчанию.
Если ставим на Windows, лучше устанавливать PostgreSQL в папку **вне** `C:\Program Files` (у меня `C:\Graf\PostgreSQL\18`), иначе `initdb` может упасть с ошибкой прав доступа. После установки стоит добавить путь к `bin` PostgreSQL в переменную среды `PATH` (нужно открыть «Изменение переменных среды текущего пользователя», в переменной `Path` добавить путь: `C:\Graf\PostgreSQL\18\bin`).

```bash
psql --version
```

## Создание базы данных

Подключаемся к PostgreSQL через `psql` (или pgAdmin) и создаем базу:

```sql
CREATE DATABASE tictactoe;
\l
```

## Настройка строки подключения

Строка подключения хранится в двух файлах:

- `TicTacToe.Server/appsettings.json`
- `TicTacToe.Server/appsettings.Development.json`

Добавляем в оба файла секцию ConnectionStrings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tictactoe;Username=postgres"
  }
}
```

## Хранение пароля через User Secrets

Пароль от PostgreSQL сохраняется в защищённом хранилище .NET (User Secrets), которое находится вне папки проекта и не попадает в Git.
Выполняем из корня решения:

```bash
dotnet user-secrets init --project TicTacToe.Server
dotnet user-secrets set "DbPassword" "пароль" --project TicTacToe.Server
dotnet user-secrets list --project TicTacToe.Server
```

User Secrets работают только в среде Development. В Docker и продакшене пароль передаётся через переменные окружения.

## Сборка строки подключения в коде

В `TicTacToe.Server/DI/Configuration.cs` строка подключения собирается так:

- Берётся `ConnectionStrings:DefaultConnection` из конфигурации.
- Если в конфигурации есть `DbPassword` (из User Secrets), он добавляется через `NpgsqlConnectionStringBuilder`.
- Зарегистрированный `AppDbContext` использует `UseNpgsql(connectionString)`.

Это позволяет держать пароль вне Git и при этом использовать одну и ту же строку подключения в разных средах.

## Миграции

Для создания и применения миграций используется `dotnet-ef` версии **8.0.11** (должна совпадать с версией EF Core в проекте).

Установка:

```bash
dotnet tool install --global dotnet-ef --version 8.0.11
```

Создание миграции:

```bash
dotnet ef migrations add InitialCreate --project TicTacToe.Server
```

Применение миграции к базе:

```bash
dotnet ef database update --project TicTacToe.Server
```

После применения в базе `tictactoe` появятся таблицы `Users` и `__EFMigrationsHistory`.

```bash
psql -h localhost -p 5432 -U postgres -d tictactoe
```

```sql
\dt
\d "Users"
```

## Docker

В `docker-compose.yml` строка подключения передаётся через переменную окружения и переопределяет значения из `appsettings.json`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=tictactoe;Username=tictactoe;Password=secret
```

User Secrets не используются. Пароль задаётся в `docker-compose.yml` (или через секреты CI/CD).

