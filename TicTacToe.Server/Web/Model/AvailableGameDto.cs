namespace TicTacToe.Server.Web.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель доступной игры для списка.</summary>
public class AvailableGameDto
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; set; }

    /// <summary>Логин создателя игры.</summary>
    public string CreatorLogin { get; set; } = null!;

    /// <summary>Символ создателя (X или O).</summary>
    public CellState CreatorSymbol { get; set; }

    /// <summary>Дата и время создания игры.</summary>
    public DateTime CreatedAt { get; set; }
}