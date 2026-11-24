namespace TicTacToe.Server.Datasource.Repository;

using TicTacToe.Server.Datasource.Model;

/// <summary>Интерфейс репозитория для работы с играми.</summary>
public interface IGameRepository
{
    /// <summary>Сохраняет игру.</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    void Save(GameEntity game);

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    GameEntity? GetById(Guid id);

    /// <summary>Возвращает список доступных игр (ожидание игроков), исключая игры указанного пользователя.</summary>
    /// <param name="excludeUserId">UUID пользователя, чьи игры нужно исключить.</param>
    /// <returns>Список моделей игр.</returns>
    List<GameEntity> GetAvailableGames(Guid? excludeUserId = null);

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список моделей игр.</returns>
    List<GameEntity> GetUserGames(Guid? userId = null);
}