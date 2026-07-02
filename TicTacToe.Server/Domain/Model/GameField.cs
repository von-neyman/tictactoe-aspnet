namespace TicTacToe.Server.Domain.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Игровое поле для крестиков-ноликов.</summary>
public class GameField
{
    /// <summary>Размер игрового поля.</summary>
    public const int Size = 3;

    /// <summary>Матрица клеток игрового поля. Cells[row][col] — доступ к клетке по строке и столбцу.</summary>
    public CellState[][] Cells { get; private set; }

    /// <summary>Создаёт пустое игровое поле (все клетки Empty).</summary>
    public GameField()
    {
        Cells = new CellState[Size][];
        for (int i = 0; i < Size; i++)
        {
            Cells[i] = new CellState[Size];
            for (int j = 0; j < Size; j++) Cells[i][j] = CellState.Empty;
        }
    }

    /// <summary>Создаёт игровое поле с заданной матрицей клеток (для десериализации).</summary>
    /// <param name="cells">Матрица клеток.</param>
    public GameField(CellState[][] cells)
    {
        Cells = cells;
    }

    /// <summary>Создаёт глубокую копию игрового поля.</summary>
    /// <returns>Копия игрового поля с идентичным содержимым.</returns>
    public GameField Clone()
    {
        var clone = new GameField();
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++) clone.Cells[i][j] = Cells[i][j];
        return clone;
    }
}