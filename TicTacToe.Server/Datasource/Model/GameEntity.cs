namespace TicTacToe.Server.Datasource.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель игры для слоя данных.</summary>
public class GameEntity
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; set; }

    /// <summary>Состояние игрового поля.</summary>
    public GameFieldEntity Field { get; set; } = null!;

    /// <summary>Символ игрока (X или O).</summary>
    public CellState PlayerSymbol { get; set; }

    /// <summary>Символ компьютера (X или O).</summary>
    public CellState ComputerSymbol { get; set; }

    /// <summary>Сложность игры.</summary>
    public int Difficulty { get; set; }

    /// <summary>Текущее состояние игры.</summary>
    public GameState State { get; set; }
}