namespace TicTacToe.Server.Domain.Repository;

using TicTacToe.Server.Domain.Model;

/// <summary>Интерфейс репозитория для работы с пользователями.</summary>
public interface IUserRepository
{
    /// <summary>Добавляет нового пользователя.</summary>
    /// <param name="user">Доменная модель пользователя.</param>
    void Add(User user);

    /// <summary>Возвращает пользователя по логину или null, если не найден.</summary>
    /// <param name="login">Логин пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    User? GetByLogin(string login);

    /// <summary>Возвращает пользователя по идентификатору или null, если не найден.</summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    User? GetById(Guid id);
}