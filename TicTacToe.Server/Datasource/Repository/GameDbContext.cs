namespace TicTacToe.Server.Datasource.Repository;

using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Datasource.Model;
using TicTacToe.Server.Domain.Enum;

/// <summary>Контекст базы данных игры.</summary>
public class GameDbContext : DbContext
{
    /// <summary>Конструктор контекста.</summary>
    /// <param name="options">Опции контекста.</param>
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    /// <summary>Пользователи.</summary>
    public DbSet<UserEntity> Users { get; set; } = null!;

    /// <summary>Игры.</summary>
    public DbSet<GameEntity> Games { get; set; } = null!;

    /// <summary>Сохраняет игру (добавляет или обновляет).</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    internal void Save(GameEntity game)
    {
        var existing = GetById(game.Id);
        if (existing == null) Games.Add(game);
        else Entry(existing).CurrentValues.SetValues(game);
        SaveChanges();
    }

    /// <summary>Возвращает игру по идентификатору или null.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    internal GameEntity? GetById(Guid id) => Games.Find(id);

    /// <summary>Возвращает список доступных игр (ожидание игроков), исключая игры указанного пользователя.</summary>
    /// <param name="excludeUserId">UUID пользователя, чьи игры нужно исключить (создатель).</param>
    internal List<GameEntity> GetAvailableGames(Guid? excludeUserId = null)
    {
        var query = Games.Where(g => g.State == GameState.WaitingForPlayers);
        if (excludeUserId.HasValue) query = query.Where(g => g.CreatorId != excludeUserId);
        return query.ToList();
    }

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    internal List<GameEntity> GetUserGames(Guid? userId = null)
    {
        var query = Games.Where(g => g.State != GameState.Draw && g.State != GameState.PlayerWon);
        if (userId.HasValue) query = query.Where(g => g.PlayerXId == userId || g.PlayerOId == userId);
        return query.ToList();
    }
}