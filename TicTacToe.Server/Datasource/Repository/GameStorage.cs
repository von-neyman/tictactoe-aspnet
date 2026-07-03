namespace TicTacToe.Server.Datasource.Repository;

using System.Collections.Concurrent;
using TicTacToe.Server.Datasource.Model;

/// <summary>Потокобезопасное хранилище текущих игр в памяти.</summary>
internal class GameStorage
{
    /// <summary>Словарь игр: ключ — Guid игры, значение — GameEntity.</summary>
    private readonly ConcurrentDictionary<Guid, GameEntity> games = new();

    /// <summary>Сохраняет игру в хранилище (добавляет или обновляет).</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    internal void Save(GameEntity game)
    {
        games[game.Id] = game;
    }

    /// <summary>Возвращает игру по идентификатору или null, если игра не найдена.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    internal GameEntity? GetById(Guid id)
    {
        games.TryGetValue(id, out var game);
        return game;
    }
}