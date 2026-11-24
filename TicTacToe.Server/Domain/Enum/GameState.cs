namespace TicTacToe.Server.Domain.Enum;

/// <summary>Состояние игры.</summary>
public enum GameState
{
    /// <summary>Ожидание второго игрока.</summary>
    WaitingForPlayers,
    /// <summary>Ход игрока (чей — в CurrentTurnPlayerId).</summary>
    PlayerTurn,
    /// <summary>Ничья.</summary>
    Draw,
    /// <summary>Победа игрока (кто — в WinnerPlayerId).</summary>
    PlayerWon
}