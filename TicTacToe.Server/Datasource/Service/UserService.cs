namespace TicTacToe.Server.Datasource.Service;

using System.Text;
using Microsoft.AspNetCore.Identity;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Repository;
using TicTacToe.Server.Domain.Service;

/// <summary>Реализация сервиса пользователей с хешированием паролей.</summary>
internal class UserService : IUserService
{
    private readonly IUserRepository repository;
    private readonly PasswordHasher<User> passwordHasher;

    /// <summary>Принимает репозиторий через конструктор.</summary>
    /// <param name="repository">Репозиторий пользователей.</param>
    public UserService(IUserRepository repository)
    {
        this.repository = repository;
        this.passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="request">Логин и пароль.</param>
    /// <returns>true, если регистрация успешна; false, если логин занят.</returns>
    public bool Register(SignUpRequest request)
    {
        if (repository.GetByLogin(request.Login) != null) return false;

        var userForHashing = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            PasswordHash = string.Empty
        };

        var user = new User
        {
            Id = userForHashing.Id,
            Login = userForHashing.Login,
            PasswordHash = passwordHasher.HashPassword(userForHashing, request.Password)
        };

        repository.Add(user);
        return true;
    }

    /// <summary>Проверяет заголовок Authorization и возвращает UUID пользователя.</summary>
    /// <param name="authorizationHeader">Значение заголовка Authorization.</param>
    /// <returns>UUID пользователя или null.</returns>
    public Guid? Authenticate(string authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return null;

        var encoded = authorizationHeader.Substring("Basic ".Length).Trim();
        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }
        catch
        {
            return null;
        }

        var index = decoded.IndexOf(':');
        if (index < 0) return null;

        var login = decoded.Substring(0, index);
        var password = decoded.Substring(index + 1);

        var user = repository.GetByLogin(login);
        if (user == null) return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return null;

        return user.Id;
    }

    /// <summary>Возвращает пользователя по идентификатору или null.</summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Доменная модель пользователя или null.</returns>
    public User? GetById(Guid id) => repository.GetById(id);
}