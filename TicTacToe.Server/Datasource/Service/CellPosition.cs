namespace TicTacToe.Server.Datasource.Service;

/// <summary>Позиция клетки на игровом поле.</summary>
internal struct CellPosition
{
    /// <summary>Строка (0-2).</summary>
    internal int Row { get; init; }

    /// <summary>Столбец (0-2).</summary>
    internal int Col { get; init; }

    /// <summary>Создаёт позицию клетки.</summary>
    /// <param name="row">Строка.</param>
    /// <param name="col">Столбец.</param>
    internal CellPosition(int row, int col)
    {
        Row = row;
        Col = col;
    }
}