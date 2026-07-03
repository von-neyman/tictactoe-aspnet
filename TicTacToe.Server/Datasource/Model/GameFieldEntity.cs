namespace TicTacToe.Server.Datasource.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель игрового поля для слоя данных.</summary>
public class GameFieldEntity
{
    /// <summary>Матрица клеток игрового поля.</summary>
    public CellState[][] Cells { get; set; } = null!;
}