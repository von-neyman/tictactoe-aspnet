namespace TicTacToe.Client.Model;

/// <summary>Запрос на регистрацию пользователя.</summary>
public class SignUpRequest
{
    /// <summary>Логин пользователя.</summary>
    public string Login { get; set; } = null!;

    /// <summary>Пароль пользователя.</summary>
    public string Password { get; set; } = null!;
}