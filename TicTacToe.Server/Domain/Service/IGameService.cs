namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;

/// <summary>Сервис игровой логики: ход компьютера, валидация хода игрока, проверка окончания игры.</summary>
internal interface IGameService
{
    /// <summary>Выполняет ход компьютера алгоритмом Минимакс и возвращает обновлённую игру.</summary>
    /// <param name="game">Текущая игра с ходом игрока.</param>
    /// <returns>Игра с ходом компьютера.</returns>
    Game MakeComputerMove(Game game);

    /// <summary>Проверяет, что игрок сделал корректный ход: ровно одна новая клетка, старые не изменены, клетка была пуста.</summary>
    /// <param name="game">Текущая игра до хода игрока.</param>
    /// <param name="updatedField">Состояние поля после хода игрока.</param>
    /// <returns>true, если ход корректен.</returns>
    bool ValidatePlayerMove(Game game, GameField updatedField);

    /// <summary>Возвращает текущее состояние игры: продолжается, ничья, победа X или победа O.</summary>
    /// <param name="field">Состояние игрового поля.</param>
    /// <returns>Состояние игры.</returns>
    GameState GetGameState(GameField field);
}