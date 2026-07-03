namespace TicTacToe.Server.Web.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель игрового поля для взаимодействия с клиентом.</summary>
public class GameFieldDto
{
    /// <summary>Матрица клеток игрового поля.</summary>
    public CellState[][] Cells { get; set; } = null!;
}