namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;

/// <summary>Интерфейс сервиса игровой логики: ход компьютера, валидация хода игрока, проверка окончания игры.</summary>
public interface IGameService
{
    /// <summary>Выполняет ход компьютера алгоритмом Минимакс и сохраняет результат в хранилище.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    void MakeComputerMove(Guid gameId);

    /// <summary>Проверяет, что игрок сделал корректный ход: ровно одна новая клетка, старые не изменены, клетка была пуста.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerField">Состояние поля после хода игрока.</param>
    /// <returns>true, если ход корректен.</returns>
    bool ValidatePlayerMove(Guid gameId, GameField playerField);

    /// <summary>Возвращает текущее состояние игры: продолжается, ничья, победа X или победа O.</summary>
    /// <param name="field">Состояние игрового поля.</param>
    /// <returns>Состояние игры.</returns>
    GameState GetGameState(GameField field);

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Доменная модель игры или null.</returns>
    Game? GetGame(Guid gameId);

    /// <summary>Создаёт новую игру, сохраняет в хранилище и возвращает.</summary>
    /// <param name="playerSymbol">Символ, выбранный игроком.</param>
    /// <returns>Новая игра.</returns>
    Game NewGame(CellState playerSymbol);
}