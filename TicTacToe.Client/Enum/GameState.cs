namespace TicTacToe.Client.Enum;

/// <summary>Состояние игры.</summary>
public enum GameState
{
    /// <summary>Игра продолжается.</summary>
    InProgress,
    /// <summary>Победили крестики.</summary>
    XWon,
    /// <summary>Победили нолики.</summary>
    OWon,
    /// <summary>Ничья.</summary>
    Draw
}