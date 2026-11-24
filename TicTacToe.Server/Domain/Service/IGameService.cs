namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;

/// <summary>Интерфейс сервиса игровой логики: создание игр, ходы, список игр.</summary>
public interface IGameService
{
    /// <summary>Создаёт новую игру (с компьютером или с человеком).</summary>
    /// <param name="creatorId">UUID создателя игры.</param>
    /// <param name="creatorSymbol">Символ создателя (X или O).</param>
    /// <param name="difficulty">Сложность игры с компьютером (null — игра с человеком).</param>
    /// <returns>Созданная игра.</returns>
    Game NewGame(Guid creatorId, CellState creatorSymbol, int? difficulty = null);

    /// <summary>Присоединяет игрока к игре.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerId">UUID присоединяющегося игрока.</param>
    /// <returns>true, если присоединение успешно.</returns>
    bool JoinGame(Guid gameId, Guid playerId);

    /// <summary>Возвращает список доступных игр (ожидание игроков).</summary>
    /// <param name="excludeUserId">UUID пользователя, чьи игры нужно исключить (создатель).</param>
    /// <returns>Список доступных игр.</returns>
    List<Game> GetAvailableGames(Guid? excludeUserId = null);

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    /// <param name="userId">UUID пользователя.</param>
    /// <returns>Список игр.</returns>
    List<Game> GetUserGames(Guid? userId = null);

    /// <summary>Делает ход игрока и (если нужно) ход компьютера.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="playerId">UUID игрока, делающего ход.</param>
    /// <param name="playerField">Состояние поля после хода.</param>
    /// <returns>true, если ход успешен.</returns>
    bool MakeMove(Guid gameId, Guid playerId, GameField playerField);

    /// <summary>Возвращает игру по идентификатору или null.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Доменная модель игры или null.</returns>
    Game? GetGame(Guid gameId);
}