namespace TicTacToe.Client.Model;

/// <summary>Модель пользователя для ответа клиенту.</summary>
public class UserDto
{
    /// <summary>Уникальный идентификатор пользователя.</summary>
    public Guid Id { get; set; }

    /// <summary>Логин пользователя.</summary>
    public string Login { get; set; } = null!;
}