namespace TicTacToe.Server.Domain.Service;

using TicTacToe.Server.Domain.Model;

/// <summary>Сервис аутентификации: регистрация и проверка учётных данных.</summary>
public interface IAuthService
{
    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="request">Логин и пароль.</param>
    /// <returns>true, если регистрация успешна; false, если логин занят.</returns>
    bool Register(SignUpRequest request);

    /// <summary>Проверяет заголовок Authorization и возвращает UUID пользователя.</summary>
    /// <param name="authorizationHeader">Значение заголовка Authorization.</param>
    /// <returns>UUID пользователя или null.</returns>
    Guid? Authenticate(string authorizationHeader);
}