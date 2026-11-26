namespace TicTacToe.Client.Model;

using TicTacToe.Client.Enum;

/// <summary>Запрос на создание новой игры.</summary>
public class NewGameRequest
{
    /// <summary>Символ, выбранный игроком (X или O).</summary>
    public CellState PlayerSymbol { get; set; }

    /// <summary>Сложность игры (null — игра с человеком, иначе — с компьютером).</summary>
    public int? Difficulty { get; set; }
}