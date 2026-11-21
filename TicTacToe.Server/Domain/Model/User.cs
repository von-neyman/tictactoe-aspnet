namespace TicTacToe.Server.Domain.Model;

/// <summary>Доменная модель пользователя.</summary>
public class User
{
    /// <summary>Уникальный идентификатор пользователя.</summary>
    public Guid Id { get; init; }

    /// <summary>Логин пользователя.</summary>
    public string Login { get; init; } = null!;

    /// <summary>Хеш пароля.</summary>
    public string PasswordHash { get; init; } = null!;
}