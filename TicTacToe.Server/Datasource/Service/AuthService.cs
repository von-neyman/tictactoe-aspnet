namespace TicTacToe.Server.Datasource.Service;

using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Service;

/// <summary>Реализация сервиса аутентификации через UserService.</summary>
internal class AuthService : IAuthService
{
    private readonly IUserService userService;

    /// <summary>Принимает UserService через конструктор.</summary>
    /// <param name="userService">Сервис пользователей.</param>
    public AuthService(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="request">Логин и пароль.</param>
    /// <returns>true, если регистрация успешна; false, если логин занят.</returns>
    public bool Register(SignUpRequest request) => userService.Register(request);

    /// <summary>Проверяет заголовок Authorization и возвращает UUID пользователя.</summary>
    /// <param name="authorizationHeader">Значение заголовка Authorization.</param>
    /// <returns>UUID пользователя или null.</returns>
    public Guid? Authenticate(string authorizationHeader) => userService.Authenticate(authorizationHeader);
}