namespace TicTacToe.Server.Web.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Модель игры для взаимодействия с клиентом.</summary>
public class GameDto
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; set; }

    /// <summary>UUID создателя игры.</summary>
    public Guid CreatorId { get; set; }

    /// <summary>Текущее состояние игрового поля.</summary>
    public GameFieldDto Field { get; set; } = null!;

    /// <summary>Тип игрока в слоте X.</summary>
    public PlayerType PlayerXType { get; set; }

    /// <summary>UUID игрока в слоте X.</summary>
    public Guid? PlayerXId { get; set; }

    /// <summary>Тип игрока в слоте O.</summary>
    public PlayerType PlayerOType { get; set; }

    /// <summary>UUID игрока в слоте O.</summary>
    public Guid? PlayerOId { get; set; }

    /// <summary>Сложность игры с компьютером (null — игра с человеком).</summary>
    public int? Difficulty { get; set; }

    /// <summary>Текущее состояние игры.</summary>
    public GameState State { get; set; }

    /// <summary>UUID игрока, чей сейчас ход.</summary>
    public Guid? CurrentTurnPlayerId { get; set; }

    /// <summary>UUID победителя.</summary>
    public Guid? WinnerPlayerId { get; set; }

    /// <summary>Дата и время создания игры.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата и время последнего хода.</summary>
    public DateTime LastMoveAt { get; set; }

    /// <summary>Количество сделанных ходов.</summary>
    public int MoveCount { get; set; }
}