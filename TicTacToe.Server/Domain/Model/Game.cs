namespace TicTacToe.Server.Domain.Model;

/// <summary>Текущая игра в крестики-нолики.</summary>
public class Game
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; init; }

    /// <summary>Текущее состояние игрового поля.</summary>
    public GameField Field { get; init; }

    /// <summary>Создаёт новую игру с уникальным идентификатором и пустым полем.</summary>
    public Game()
    {
        Id = Guid.NewGuid();
        Field = new GameField();
    }

    /// <summary>Создаёт игру с заданным идентификатором и состоянием поля.</summary>
    /// <param name="id">Уникальный идентификатор игры.</param>
    /// <param name="field">Состояние игрового поля.</param>
    public Game(Guid id, GameField field)
    {
        Id = id;
        Field = field;
    }
}