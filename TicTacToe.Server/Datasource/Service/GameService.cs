namespace TicTacToe.Server.Datasource.Service;

using TicTacToe.Server.Datasource.Mapper;
using TicTacToe.Server.Datasource.Repository;
using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Service;

/// <summary>Реализация игровой логики: создание игр, ходы, список игр.</summary>
internal class GameService : IGameService
{
    /// <summary>Репозиторий для доступа к хранилищу игр.</summary>
    private readonly IGameRepository repository;

    /// <summary>Принимает репозиторий через конструктор.</summary>
    /// <param name="repository">Репозиторий для работы с хранилищем.</param>
    public GameService(IGameRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>Создаёт новую игру (с компьютером или с человеком).</summary>
    /// <param name="creatorId">UUID создателя игры.</param>
    /// <param name="creatorSymbol">Символ создателя (X или O).</param>
    /// <param name="difficulty">Сложность игры с компьютером (null — игра с человеком).</param>
    /// <returns>Созданная игра.</returns>
    public Game NewGame(Guid creatorId, CellState creatorSymbol, int? difficulty = null)
    {
        var game = new Game(creatorId, creatorSymbol, difficulty);
        if (game.PlayerXType == PlayerType.Computer) game = MakeComputerMove(game);
        repository.Save(DatasourceMapper.GameToEntity(game));
        return game;
    }

    /// <summary>Присоединяет игрока к игре.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerId">UUID присоединяющегося игрока.</param>
    /// <returns>true, если присоединение успешно.</returns>
    public bool JoinGame(Guid gameId, Guid playerId)
    {
        var game = GetGame(gameId);
        if (game == null) return false;
        if (game.State != GameState.WaitingForPlayers) return false;
        if (game.PlayerXId == playerId || game.PlayerOId == playerId) return false;

        var updated = game.JoinAsOpponent(playerId);
        repository.Save(DatasourceMapper.GameToEntity(updated));
        return true;
    }

    /// <summary>Возвращает список доступных игр (ожидание игроков).</summary>
    /// <param name="excludeUserId">UUID пользователя, чьи игры нужно исключить (создатель).</param>
    /// <returns>Список доступных игр.</returns>
    public List<Game> GetAvailableGames(Guid? excludeUserId = null)
    {
        var entities = repository.GetAvailableGames(excludeUserId);
        return entities.Select(DatasourceMapper.GameToDomain).ToList();
    }

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    /// <param name="userId">UUID пользователя.</param>
    /// <returns>Список игр.</returns>
    public List<Game> GetUserGames(Guid? userId = null)
    {
        var entities = repository.GetUserGames(userId);
        return entities.Select(DatasourceMapper.GameToDomain).ToList();
    }

    /// <summary>Делает ход игрока и (если нужно) ход компьютера.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerId">UUID игрока, делающего ход.</param>
    /// <param name="playerField">Состояние поля после хода.</param>
    /// <returns>true, если ход успешен.</returns>
    public bool MakeMove(Guid gameId, Guid playerId, GameField playerField)
    {
        var game = GetGame(gameId);
        if (game == null) return false;
        if (game.State != GameState.PlayerTurn && !(game.State == GameState.WaitingForPlayers && game.PlayerXId == playerId)) return false;
        if (game.CurrentTurnPlayerId != playerId) return false;
        if (!ValidatePlayerMove(game, playerField)) return false;

        var updated = ApplyMove(game, playerField);
        if (IsComputerTurn(updated)) updated = MakeComputerMove(updated);

        repository.Save(DatasourceMapper.GameToEntity(updated));
        return true;
    }

    /// <summary>Возвращает игру по идентификатору или null.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Доменная модель игры или null.</returns>
    public Game? GetGame(Guid gameId)
    {
        var gameEntity = repository.GetById(gameId);
        if (gameEntity == null) return null;
        return DatasourceMapper.GameToDomain(gameEntity);
    }

    /// <summary>Применяет ход к игре и возвращает обновлённую игру.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <param name="newField">Состояние поля после хода.</param>
    /// <returns>Обновлённая игра.</returns>
    private static Game ApplyMove(Game game, GameField newField)
    {
        var newState = GetGameState(game, newField);
        var winnerId = newState == GameState.PlayerWon ? GetWinnerId(game, newField) : null;
        var nextTurnPlayerId = newState == GameState.PlayerTurn ? GetOpponentId(game) : null;
        return game.UpdateField(newField, newState, nextTurnPlayerId, winnerId);
    }

    /// <summary>Делает ход компьютера.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <returns>Игра с ходом компьютера.</returns>
    private Game MakeComputerMove(Game game)
    {
        var bestMove = FindBestMove(game);
        var computerSymbol = GetComputerSymbol(game);
        var newField = game.Field.Clone();
        newField.Cells[bestMove.Row][bestMove.Col] = computerSymbol;

        return ApplyMove(game, newField);
    }

    /// <summary>Возвращает символ компьютера в игре.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <returns>Символ компьютера (X или O).</returns>
    private static CellState GetComputerSymbol(Game game) => game.PlayerXType == PlayerType.Computer ? CellState.X : CellState.O;

    /// <summary>Возвращает UUID противника игрока.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <returns>UUID противника (или null).</returns>
    private static Guid? GetOpponentId(Game game) => game.PlayerXId == game.CurrentTurnPlayerId ? game.PlayerOId : game.PlayerXId;

    /// <summary>Проверяет, что ход игрока корректен: ровно одна новая клетка, старые не изменены, клетка была пуста.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <param name="playerField">Состояние поля после хода.</param>
    /// <returns>true, если ход корректен.</returns>
    private static bool ValidatePlayerMove(Game game, GameField playerField)
    {
        var originalField = game.Field;
        var changes = 0;

        for (int i = 0; i < GameField.Size; i++)
        {
            for (int j = 0; j < GameField.Size; j++)
            {
                var oldCell = originalField.Cells[i][j];
                var newCell = playerField.Cells[i][j];
                if (oldCell == newCell) continue;

                if (oldCell != CellState.Empty || newCell == CellState.Empty) return false;
                changes++;
            }
        }

        return changes == 1;
    }

    /// <summary>Возвращает новое состояние игры после хода.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <param name="newField">Состояние поля после хода.</param>
    /// <returns>Новое состояние игры.</returns>
    private static GameState GetGameState(Game game, GameField newField)
    {
        if (game.State == GameState.WaitingForPlayers) return GameState.WaitingForPlayers;
        var winner = GetWinner(newField);
        if (winner == CellState.X || winner == CellState.O) return GameState.PlayerWon;
        if (IsBoardFull(newField)) return GameState.Draw;
        return GameState.PlayerTurn;
    }

    /// <summary>Возвращает UUID победителя (если есть) или null.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <param name="field">Состояние поля.</param>
    /// <returns>UUID победителя или null.</returns>
    private static Guid? GetWinnerId(Game game, GameField field)
    {
        var winner = GetWinner(field);
        if (winner == CellState.X) return game.PlayerXId;
        if (winner == CellState.O) return game.PlayerOId;
        return null;
    }

    /// <summary>Проверяет, ходит лм сейчас компьютер.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <returns>true, если ход компьютера.</returns>
    private static bool IsComputerTurn(Game game) => game.State == GameState.PlayerTurn && game.CurrentTurnPlayerId == null;

    /// <summary>Возвращает победителя (X или O) или null, если победителя нет.</summary>
    /// <param name="field">Состояние игрового поля.</param>
    /// <returns>X, O или null.</returns>
    private static CellState? GetWinner(GameField field)
    {
        var cells = field.Cells;

        for (int row = 0; row < GameField.Size; row++)
        {
            if (cells[row][0] != CellState.Empty &&
                cells[row][0] == cells[row][1] &&
                cells[row][1] == cells[row][2])
                return cells[row][0];
        }

        for (int col = 0; col < GameField.Size; col++)
        {
            if (cells[0][col] != CellState.Empty &&
                cells[0][col] == cells[1][col] &&
                cells[1][col] == cells[2][col])
                return cells[0][col];
        }

        if (cells[0][0] != CellState.Empty &&
            cells[0][0] == cells[1][1] &&
            cells[1][1] == cells[2][2])
            return cells[0][0];

        if (cells[0][2] != CellState.Empty &&
            cells[0][2] == cells[1][1] &&
            cells[1][1] == cells[2][0])
            return cells[0][2];

        return null;
    }

    /// <summary>Проверяет, все ли клетки игрового поля заняты.</summary>
    /// <param name="field">Состояние игрового поля.</param>
    /// <returns>true, если свободных клеток нет.</returns>
    private static bool IsBoardFull(GameField field)
    {
        for (int i = 0; i < GameField.Size; i++)
            for (int j = 0; j < GameField.Size; j++)
                if (field.Cells[i][j] == CellState.Empty) return false;
        return true;
    }

    /// <summary>Находит лучший ход для компьютера, выбирая случайный среди ходов с одинаковой максимальной оценкой.</summary>
    /// <param name="game">Текущая игра.</param>
    /// <returns>Лучшая позиция для хода.</returns>
    private CellPosition FindBestMove(Game game)
    {
        var bestScore = -1;
        var bestMoves = new List<CellPosition>();
        var computerSymbol = GetComputerSymbol(game);
        var playerSymbol = computerSymbol == CellState.X ? CellState.O : CellState.X;

        for (int i = 0; i < GameField.Size; i++)
        {
            for (int j = 0; j < GameField.Size; j++)
            {
                if (game.Field.Cells[i][j] != CellState.Empty) continue;
                var testField = game.Field.Clone();
                testField.Cells[i][j] = computerSymbol;
                var score = Minimax(testField, 1, false, game, computerSymbol, playerSymbol);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(new CellPosition(i, j));
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(new CellPosition(i, j));
                }
            }
        }
        return bestMoves[Random.Shared.Next(bestMoves.Count)];
    }

    /// <summary>Рекурсивный алгоритм Минимакс для оценки позиции.</summary>
    /// <param name="field">Текущее состояние поля.</param>
    /// <param name="depth">Текущая глубина рекурсии в полуходах.</param>
    /// <param name="isMaximizing">true — ход компьютера (максимизируем), false — ход игрока (минимизируем).</param>
    /// <param name="game">Текущая игра (для доступа к символам и сложности).</param>
    /// <param name="computerSymbol">Символ компьютера.</param>
    /// <param name="playerSymbol">Символ игрока.</param>
    /// <returns>Оценка позиции: 1 — победа компьютера, -1 — победа игрока, 0 — ничья.</returns>
    private static int Minimax(GameField field, int depth, bool isMaximizing, Game game, CellState computerSymbol, CellState playerSymbol)
    {
        if (depth > game.Difficulty) return 0;
        var winner = GetWinner(field);
        if (winner == computerSymbol) return 1;
        if (winner == playerSymbol) return -1;
        if (IsBoardFull(field)) return 0;
        if (isMaximizing)
        {
            var maxScore = -1;
            for (int i = 0; i < GameField.Size; i++)
            {
                for (int j = 0; j < GameField.Size; j++)
                {
                    if (field.Cells[i][j] != CellState.Empty) continue;
                    var testField = field.Clone();
                    testField.Cells[i][j] = computerSymbol;
                    var score = Minimax(testField, depth + 1, false, game, computerSymbol, playerSymbol);
                    if (score > maxScore) maxScore = score;
                }
            }
            return maxScore;
        }
        else
        {
            var minScore = 1;
            for (int i = 0; i < GameField.Size; i++)
            {
                for (int j = 0; j < GameField.Size; j++)
                {
                    if (field.Cells[i][j] != CellState.Empty) continue;
                    var testField = field.Clone();
                    testField.Cells[i][j] = playerSymbol;
                    var score = Minimax(testField, depth + 1, true, game, computerSymbol, playerSymbol);
                    if (score < minScore) minScore = score;
                }
            }
            return minScore;
        }
    }
}