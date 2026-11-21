namespace TicTacToe.Server.Domain.Model;

/// <summary>Доменная модель запроса на регистрацию.</summary>
public class SignUpRequest
{
    /// <summary>Логин пользователя.</summary>
    public string Login { get; init; } = null!;

    /// <summary>Пароль пользователя.</summary>
    public string Password { get; init; } = null!;
}