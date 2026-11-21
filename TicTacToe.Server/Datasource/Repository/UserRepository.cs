namespace TicTacToe.Server.Datasource.Repository;

using TicTacToe.Server.Datasource;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Repository;

/// <summary>Реализация репозитория пользователей через EF Core.</summary>
internal class UserRepository : IUserRepository
{
    private readonly AppDbContext context;

    /// <summary>Принимает контекст базы данных через конструктор.</summary>
    /// <param name="context">Контекст базы данных.</param>
    public UserRepository(AppDbContext context)
    {
        this.context = context;
    }

    /// <summary>Добавляет нового пользователя в базу данных.</summary>
    /// <param name="user">Доменная модель пользователя.</param>
    public void Add(User user)
    {
        var entity = new Model.UserEntity
        {
            Id = user.Id,
            Login = user.Login,
            PasswordHash = user.PasswordHash
        };
        context.Users.Add(entity);
        context.SaveChanges();
    }

    /// <summary>Возвращает пользователя по логину или null.</summary>
    /// <param name="login">Логин пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    public User? GetByLogin(string login)
    {
        var entity = context.Users.FirstOrDefault(u => u.Login == login);
        return entity == null ? null : ToDomain(entity);
    }

    /// <summary>Возвращает пользователя по идентификатору или null.</summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    public User? GetById(Guid id)
    {
        var entity = context.Users.Find(id);
        return entity == null ? null : ToDomain(entity);
    }

    /// <summary>Преобразует сущность базы данных в доменную модель.</summary>
    /// <param name="entity">Сущность пользователя.</param>
    /// <returns>Доменная модель пользователя.</returns>
    private static User ToDomain(Model.UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Login = entity.Login,
            PasswordHash = entity.PasswordHash
        };
    }
}