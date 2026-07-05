namespace TicTacToe.Server.Datasource.Service;

using TicTacToe.Server.Datasource.Mapper;
using TicTacToe.Server.Datasource.Repository;
using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Service;

/// <summary>Реализация игровой логики: Минимакс, валидация хода игрока, проверка окончания игры.</summary>
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

    /// <summary>Выполняет ход компьютера алгоритмом Минимакс и сохраняет результат в хранилище.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    public void MakeComputerMove(Guid gameId)
    {
        var game = GetGame(gameId)!;
        var bestMove = FindBestMove(game);
        var newField = game.Field.Clone();
        newField.Cells[bestMove.Row][bestMove.Col] = game.ComputerSymbol;
        var newState = GetGameState(newField);
        var updatedGame = game.UpdateField(newField, newState);
        repository.Save(DatasourceMapper.GameToEntity(updatedGame));
    }

    /// <summary>Проверяет, что игрок сделал корректный ход и сохраняет его, если ход корректен.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerField">Состояние поля после хода игрока.</param>
    /// <returns>true, если ход корректен и сохранён.</returns>
    public bool ValidatePlayerMove(Guid gameId, GameField playerField)
    {
        var game = GetGame(gameId);
        if (game == null || game.State != GameState.InProgress) return false;
        var originalField = game.Field;
        var changes = 0;
        for (int i = 0; i < GameField.Size; i++)
        {
            for (int j = 0; j < GameField.Size; j++)
            {
                var oldCell = originalField.Cells[i][j];
                var newCell = playerField.Cells[i][j];
                if (oldCell == newCell) continue;
                if (oldCell != CellState.Empty || newCell != game.PlayerSymbol) return false;
                changes++;
            }
        }
        var isValid = changes == 1;
        if (isValid)
        {
            var newState = GetGameState(playerField);
            var updatedGame = game.UpdateField(playerField, newState);
            repository.Save(DatasourceMapper.GameToEntity(updatedGame));
        }
        return isValid;
    }

    /// <summary>Возвращает текущее состояние игры: продолжается, ничья, победа X или победа O.</summary>
    /// <param name="field">Состояние игрового поля.</param>
    /// <returns>Состояние игры.</returns>
    public GameState GetGameState(GameField field)
    {
        var winner = GetWinner(field);
        if (winner == CellState.X) return GameState.XWon;
        if (winner == CellState.O) return GameState.OWon;
        if (IsBoardFull(field)) return GameState.Draw;
        return GameState.InProgress;
    }

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Доменная модель игры или null.</returns>
    public Game? GetGame(Guid gameId)
    {
        var gameEntity = repository.GetById(gameId);
        if (gameEntity == null) return null;
        return DatasourceMapper.GameToDomain(gameEntity);
    }

    /// <summary>Создаёт новую игру, сохраняет в хранилище и возвращает. Если компьютер играет за X — сразу делает первый ход.</summary>
    /// <param name="playerSymbol">Символ, выбранный игроком.</param>
    /// <param name="difficulty">Сложность игры.</param>
    /// <returns>Новая игра.</returns>
    public Game NewGame(CellState playerSymbol, int difficulty)
    {
        var game = new Game(playerSymbol, difficulty);
        repository.Save(DatasourceMapper.GameToEntity(game));
        if (game.ComputerSymbol == CellState.X) MakeComputerMove(game.Id);
        return GetGame(game.Id)!;
    }

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
        for (int i = 0; i < GameField.Size; i++)
        {
            for (int j = 0; j < GameField.Size; j++)
            {
                if (game.Field.Cells[i][j] != CellState.Empty) continue;
                var testField = game.Field.Clone();
                testField.Cells[i][j] = game.ComputerSymbol;
                var score = Minimax(testField, 1, false, game);
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
    /// <returns>Оценка позиции: 1 — победа компьютера, -1 — победа игрока, 0 — ничья.</returns>
    private static int Minimax(GameField field, int depth, bool isMaximizing, Game game)
    {
        if (depth > game.Difficulty) return 0;
        var winner = GetWinner(field);
        if (winner == game.ComputerSymbol) return 1;
        if (winner == game.PlayerSymbol) return -1;
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
                    testField.Cells[i][j] = game.ComputerSymbol;
                    var score = Minimax(testField, depth + 1, false, game);
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
                    testField.Cells[i][j] = game.PlayerSymbol;
                    var score = Minimax(testField, depth + 1, true, game);
                    if (score < minScore) minScore = score;
                }
            }
            return minScore;
        }
    }
}