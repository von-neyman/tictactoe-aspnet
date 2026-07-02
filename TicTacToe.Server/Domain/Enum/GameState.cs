namespace TicTacToe.Server.Domain.Enum;

/// <summary>Состояние игры.</summary>
public enum GameState
{
    /// <summary>Игра продолжается.</summary>
    InProgress,
    /// <summary>Ничья.</summary>
    Draw,
    /// <summary>Победили крестики.</summary>
    XWon,
    /// <summary>Победили нолики.</summary>
    OWon
}