namespace TicTacToe.Server.Datasource.Repository;

using TicTacToe.Server.Datasource.Model;

/// <summary>Реализация репозитория игр через EF Core.</summary>
internal class GameRepository : IGameRepository
{
    private readonly GameDbContext context;

    /// <summary>Принимает контекст базы данных через конструктор.</summary>
    /// <param name="context">Контекст базы данных.</param>
    public GameRepository(GameDbContext context)
    {
        this.context = context;
    }

    /// <summary>Сохраняет игру.</summary>
    /// <param name="game">Модель игры для сохранения.</param>
    public void Save(GameEntity game) => context.Save(game);

    /// <summary>Возвращает игру по идентификатору или null.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <returns>Модель игры или null.</returns>
    public GameEntity? GetById(Guid id) => context.GetById(id);

    /// <summary>Возвращает список доступных игр (ожидание игроков), исключая игры указанного пользователя.</summary>
    /// <param name="excludeUserId">UUID пользователя, чьи игры нужно исключить.</param>
    /// <returns>Список моделей игр.</returns>
    public List<GameEntity> GetAvailableGames(Guid? excludeUserId = null) => context.GetAvailableGames(excludeUserId);

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список моделей игр.</returns>
    public List<GameEntity> GetUserGames(Guid? userId = null) => context.GetUserGames(userId);
}