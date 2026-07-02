namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;

/// <summary>Реализация игровой логики: Минимакс, валидация хода игрока, проверка окончания игры.</summary>
internal class GameService : IGameService
{
    /// <summary>Выполняет ход компьютера алгоритмом Минимакс и возвращает обновлённую игру.</summary>
    /// <param name="game">Текущая игра с ходом игрока.</param>
    /// <returns>Игра с ходом компьютера.</returns>
    public Game MakeComputerMove(Game game)
    {
        var bestMove = FindBestMove(game);
        var newField = game.Field.Clone();
        newField.Cells[bestMove.Row][bestMove.Col] = game.ComputerSymbol;
        return game.UpdateField(newField);
    }

    /// <summary>Проверяет, что игрок сделал корректный ход: ровно одна новая клетка, старые не изменены, клетка была пуста.</summary>
    /// <param name="game">Текущая игра до хода игрока.</param>
    /// <param name="updatedField">Состояние поля после хода игрока.</param>
    /// <returns>true, если ход корректен.</returns>
    public bool ValidatePlayerMove(Game game, GameField updatedField)
    {
        var original = game.Field;
        var changes = 0;
        for (int i = 0; i < GameField.Size; i++)
        {
            for (int j = 0; j < GameField.Size; j++)
            {
                var oldCell = original.Cells[i][j];
                var newCell = updatedField.Cells[i][j];
                if (oldCell == newCell) continue;
                if (oldCell != CellState.Empty || newCell != game.PlayerSymbol) return false;
                changes++;
            }
        }
        return changes == 1;
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