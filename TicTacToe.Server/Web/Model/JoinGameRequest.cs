namespace TicTacToe.Server.Web.Model;

/// <summary>Запрос на присоединение к игре.</summary>
public class JoinGameRequest
{
    /// <summary>Идентификатор игры.</summary>
    public Guid GameId { get; set; }
}