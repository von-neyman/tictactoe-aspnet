namespace TicTacToe.Server.Web.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель игры для взаимодействия с клиентом.</summary>
public class GameDto
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; set; }

    /// <summary>Текущее состояние игрового поля.</summary>
    public GameFieldDto Field { get; set; } = null!;

    /// <summary>Символ игрока (X или O).</summary>
    public CellState PlayerSymbol { get; set; }

    /// <summary>Символ компьютера (X или O).</summary>
    public CellState ComputerSymbol { get; set; }

    /// <summary>Сложность игры.</summary>
    public int Difficulty { get; set; }

    /// <summary>Текущее состояние игры.</summary>
    public GameState State { get; set; }
}