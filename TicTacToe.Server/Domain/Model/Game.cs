namespace TicTacToe.Server.Domain.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Текущая игра в крестики-нолики.</summary>
public class Game
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; init; }

    /// <summary>Текущее состояние игрового поля.</summary>
    public GameField Field { get; init; }

    /// <summary>Символ, которым играет игрок (X или O).</summary>
    public CellState PlayerSymbol { get; init; }

    /// <summary>Символ, которым играет компьютер (X или O).</summary>
    public CellState ComputerSymbol { get; init; }

    /// <summary>
    /// Сложность игры: максимальная глубина просчёта ходов компьютером в полуходах (depth).
    /// 0 — компьютер не просчитывает ходы вообще, выбирает случайную пустую клетку.
    /// 1 — компьютер видит только свои ближайшие ходы (может завершить победу, но не видит угроз).
    /// 2 — видит свои ближайшие ходы и ответ игрока (может защититься от немедленного поражения).
    /// Size * Size — просчитывает всё, идеальная игра.
    /// </summary>
    public int Difficulty { get; init; }

    /// <summary>Текущее состояние игры.</summary>
    public GameState State { get; init; }

    /// <summary>Создаёт новую игру с уникальным идентификатором, пустым полем и выбранными символами.</summary>
    /// <param name="playerSymbol">Символ, выбранный игроком.</param>
    /// <param name="difficulty">Сложность игры.</param>
    public Game(CellState playerSymbol, int difficulty)
    {
        Id = Guid.NewGuid();
        Field = new GameField();
        PlayerSymbol = playerSymbol;
        ComputerSymbol = playerSymbol == CellState.X ? CellState.O : CellState.X;
        Difficulty = difficulty;
        State = GameState.InProgress;
    }

    /// <summary>Создаёт экземпляр игры с новым полем и указанным состоянием.</summary>
    /// <param name="newField">Новое состояние игрового поля.</param>
    /// <param name="newState">Новое состояние игры.</param>
    /// <returns>Новая игра с обновлённым полем и состоянием.</returns>
    public Game UpdateField(GameField newField, GameState newState)
    {
        return new Game(PlayerSymbol, Difficulty)
        {
            Id = this.Id,
            Field = newField,
            State = newState
        };
    }
}