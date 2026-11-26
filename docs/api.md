# Описание API

Описание всех endpoints сервера, включая примеры запросов через `curl`.

## Базовый URL

`http://localhost:5114`

(Порт может отличаться при запуске через терминал — смотри консоль сервера).

## Аутентификация

Все endpoints, кроме регистрации и логина, требуют заголовок `Authorization` с Basic Auth:

`Authorization: Basic base64(login:password)`

Пример: для `login=testuser`, `password=testpass` строка `base64("testuser:testpass")` = `dGVzdHVzZXI6dGVzdHBhc3M=`.

## Регистрация

**POST** `/auth/signup`

Создаёт нового пользователя. Пароль хешируется.

**Тело запроса:**

```json
{
  "login": "testuser",
  "password": "testpass"
}
```

Ответ: `200 OK` (пустое тело) или `409 Conflict`, если логин занят.

Пример:

```bash
curl -X POST http://localhost:5114/auth/signup -H "Content-Type: application/json" -d "{\"login\":\"testuser\",\"password\":\"testpass\"}"
```

## Логин

**POST** `/auth/login`

Проверяет учётные данные из заголовка `Authorization` и возвращает UUID пользователя.

Заголовок: `Authorization: Basic base64(login:password)`.

Ответ: `200 OK` с UUID или `401 Unauthorized`.

Пример:

```bash
curl -X POST http://localhost:5114/auth/login -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M="
```

Ответ:

```json
{ "id": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7" }
```

## Создание игры

**POST** `/game/new`

Создаёт новую игру. Если `difficulty` передан — игра с компьютером, иначе — с человеком (ожидание второго игрока).

**Тело запроса:**

```json
{
  "playerSymbol": 1,
  "difficulty": 9
}
```

`playerSymbol`: `1` — X, `2` — O. `difficulty`: `null` — игра с человеком, `0`, `1`, `2`, `9` — сложность компьютера.

Ответ: `200 OK` с игрой (`GameDto`).

### Пример: игра с компьютером

```bash
curl -X POST http://localhost:5114/game/new -H "Content-Type: application/json" -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M=" -d "{\"playerSymbol\":1,\"difficulty\":9}"
```

Ответ:

```json
{
  "id": "925e64eb-c61d-46a1-97f4-87c8682df85f",
  "creatorId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "field": { "cells": [[0,0,0],[0,0,0],[0,0,0]] },
  "playerXType": 1,
  "playerXId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "playerOType": 2,
  "playerOId": null,
  "difficulty": 9,
  "state": 1,
  "currentTurnPlayerId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "winnerPlayerId": null,
  "createdAt": "2026-11-25T11:21:25.743566Z",
  "lastMoveAt": "2026-11-25T11:21:25.743582Z",
  "moveCount": 0
}
```

### Пример: игра с человеком

Можно явно передать `difficulty: null`:

```bash
curl -X POST http://localhost:5114/game/new -H "Content-Type: application/json" -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M=" -d "{\"playerSymbol\":1,\"difficulty\":null}"
```

Или вообще не передавать `difficulty` — результат будет тот же:

```bash
curl -X POST http://localhost:5114/game/new -H "Content-Type: application/json" -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M=" -d "{\"playerSymbol\":1}"
```

Ответ:

```json
{
  "id": "03d0c6a5-e631-488d-b4b8-7ec2cba330ab",
  "creatorId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "field": { "cells": [[0,0,0],[0,0,0],[0,0,0]] },
  "playerXType": 1,
  "playerXId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "playerOType": 0,
  "playerOId": null,
  "difficulty": null,
  "state": 0,
  "currentTurnPlayerId": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "winnerPlayerId": null,
  "createdAt": "2026-11-25T11:23:30.7282131Z",
  "lastMoveAt": "2026-11-25T11:23:30.7282135Z",
  "moveCount": 0
}
```

Важно: `playerOType` = `0` (None), `state` = `0` (WaitingForPlayers) — игра ждёт второго игрока. Именно к этой игре можно присоединиться через `/game/join`.

## Присоединение к игре

**POST** `/game/join`

Присоединяет текущего пользователя к игре в состоянии `WaitingForPlayers`.

**Тело запроса:**

```json
{
  "gameId": "03d0c6a5-e631-488d-b4b8-7ec2cba330ab"
}
```

Ответ: `200 OK` или `400 Bad Request`.

Пример:

```bash
curl -X POST http://localhost:5114/game/join -H "Content-Type: application/json" -H "Authorization: Basic cGxheWVyMjpwYXNzMg==" -d "{\"gameId\":\"03d0c6a5-e631-488d-b4b8-7ec2cba330ab\"}"
```

## Список доступных игр

**GET** `/game/available`

Возвращает игры в состоянии `WaitingForPlayers`, где создатель — не текущий пользователь.

Ответ: `200 OK` с массивом `AvailableGameDto`.

Пример:

```bash
curl -X GET http://localhost:5114/game/available -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M="
```

Ответ:

```json
[
  {
    "id": "03d0c6a5-e631-488d-b4b8-7ec2cba330ab",
    "creatorLogin": "player2",
    "creatorSymbol": 2,
    "createdAt": "2026-11-25T11:23:30.728213Z"
  }
]
```

## Список моих игр

**GET** `/game/my`

Возвращает незавершённые игры, где текущий пользователь — участник.

Ответ: `200 OK` с массивом `GameDto`.

Пример:

```bash
curl -X GET http://localhost:5114/game/my -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M="
```

## Ход игрока

**POST** `/game/{id}/move`

Делает ход в игре. Если после хода игра продолжается и ход компьютера — компьютер отвечает автоматически.

**Параметр пути:** `id` — идентификатор игры.

**Тело запроса:** `GameFieldDto` — поле после хода.

```json
{
  "cells": [[1,0,0],[0,0,0],[0,0,0]]
}
```

Ответ: `200 OK` или `400 Bad Request` (некорректный ход).

Пример:

```bash
curl -X POST http://localhost:5114/game/925e64eb-c61d-46a1-97f4-87c8682df85f/move -H "Content-Type: application/json" -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M=" -d "{\"cells\":[[1,0,0],[0,0,0],[0,0,0]]}"
```

## Получение игры

**GET** `/game/{id}`

Возвращает актуальное состояние игры.

**Параметр пути:** `id` — идентификатор игры.

Ответ: `200 OK` с `GameDto` или `404 Not Found`.

Пример:

```bash
curl -X GET http://localhost:5114/game/925e64eb-c61d-46a1-97f4-87c8682df85f -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M="
```

## Получение информации о пользователе

**GET** `/user/{id}`

Возвращает логин пользователя по UUID.

**Параметр пути:** `id` — UUID пользователя.

Ответ: `200 OK` с `UserDto` или `404 Not Found`.

Пример:

```bash
curl -X GET http://localhost:5114/user/d18f76f0-d1d1-4eb7-8437-00f7629dafa7 -H "Authorization: Basic dGVzdHVzZXI6dGVzdHBhc3M="
```

Ответ:

```json
{
  "id": "d18f76f0-d1d1-4eb7-8437-00f7629dafa7",
  "login": "testuser"
}
```

## Коды состояния

| Код | Описание |
|-----|----------|
| 200 | Успех |
| 400 | Некорректный запрос (неверный ход, не удалось присоединиться) |
| 401 | Не авторизован (нет заголовка `Authorization` или неверные данные) |
| 404 | Ресурс не найден (игра или пользователь) |
| 409 | Конфликт (логин уже занят) |

## Формат ошибок

Все ошибки возвращаются в формате:

```json
{
  "message": "Текст ошибки"
}
```

Пример:

```json
{
  "message": "Некорректный ход."
}
```

## Перечисления (enum)

**CellState** (состояние клетки):
- `0` — Empty (пусто)
- `1` — X (крестик)
- `2` — O (нолик)

**GameState** (состояние игры):
- `0` — WaitingForPlayers (ожидание игроков)
- `1` — PlayerTurn (ход игрока)
- `2` — Draw (ничья)
- `3` — PlayerWon (победа игрока)

**PlayerType** (тип игрока):
- `0` — None (слот пуст)
- `1` — Human (человек)
- `2` — Computer (компьютер)