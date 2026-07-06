namespace TicTacToe.Client.Model;

using TicTacToe.Client.Enum;

/// <summary>Запрос на создание новой игры.</summary>
public class NewGameRequest
{
    /// <summary>Символ, выбранный игроком (X или O).</summary>
    public CellState PlayerSymbol { get; set; }

    /// <summary>Сложность игры (0, 1, 2, 9).</summary>
    public int Difficulty { get; set; }
}