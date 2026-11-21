namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Model;

/// <summary>Сервис для работы с пользователями.</summary>
public interface IUserService
{
    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="request">Логин и пароль.</param>
    /// <returns>true, если регистрация успешна; false, если логин занят.</returns>
    bool Register(SignUpRequest request);

    /// <summary>Проверяет заголовок Authorization и возвращает UUID пользователя.</summary>
    /// <param name="authorizationHeader">Значение заголовка Authorization.</param>
    /// <returns>UUID пользователя или null.</returns>
    Guid? Authenticate(string authorizationHeader);

    /// <summary>Возвращает пользователя по идентификатору или null.</summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    User? GetById(Guid id);
}