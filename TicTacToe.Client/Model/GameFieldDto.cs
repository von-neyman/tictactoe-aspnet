namespace TicTacToe.Client.Model;

using TicTacToe.Client.Enum;

/// <summary>Модель игрового поля для взаимодействия с клиентом.</summary>
public class GameFieldDto
{
    /// <summary>Матрица клеток игрового поля.</summary>
    public CellState[][] Cells { get; set; } = null!;
}