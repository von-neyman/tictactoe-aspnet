namespace TicTacToe.Server.Datasource.Repository;

using TicTacToe.Server.Datasource.Model;

/// <summary>Реализация репозитория, работающая с GameStorage.</summary>
internal class GameRepository : IGameRepository
{
    /// <summary>Хранилище игр.</summary>
    private readonly GameStorage storage;

    /// <summary>Принимает хранилище через конструктор.</summary>
    /// <param name="storage">Хранилище игр.</param>
    public GameRepository(GameStorage storage)
    {
        this.storage = storage;
    }

    /// <summary>Сохраняет игру в хранилище.</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    public void Save(GameEntity game)
    {
        storage.Save(game);
    }

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    public GameEntity? GetById(Guid id)
    {
        return storage.GetById(id);
    }
}