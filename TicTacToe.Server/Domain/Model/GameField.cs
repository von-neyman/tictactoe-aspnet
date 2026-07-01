namespace TicTacToe.Server.Domain.Model;

/// <summary>Игровое поле для крестиков-ноликов.</summary>
public class GameField
{
    /// <summary>Размер игрового поля.</summary>
    public const int Size = 3;

    /// <summary>Пустая клетка.</summary>
    public const int Empty = 0;

    /// <summary>Крестик (первый игрок).</summary>
    public const int X = 1;

    /// <summary>Нолик (второй игрок).</summary>
    public const int O = 2;

    /// <summary>Матрица клеток игрового поля. Cells[row][col] — доступ к клетке по строке и столбцу.</summary>
    public int[][] Cells { get; init; }

    /// <summary>Создаёт пустое игровое поле (все клетки Empty).</summary>
    public GameField()
    {
        Cells = new int[Size][];
        for (int i = 0; i < Size; i++) Cells[i] = new int[Size];
    }

    /// <summary>Создаёт игровое поле с заданной матрицей клеток.</summary>
    /// <param name="cells">Матрица клеток.</param>
    public GameField(int[][] cells)
    {
        Cells = cells;
    }
}