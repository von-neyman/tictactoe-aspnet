namespace TicTacToe.Server.Datasource.Repository;

using TicTacToe.Server.Datasource.Model;

/// <summary>Интерфейс репозитория для работы с хранилищем игр.</summary>
public interface IGameRepository
{
    /// <summary>Сохраняет игру в хранилище.</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    void Save(GameEntity game);

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    GameEntity? GetById(Guid id);
}