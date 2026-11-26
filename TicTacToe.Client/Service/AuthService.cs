namespace TicTacToe.Client.Service;

/// <summary>Сервис для хранения учётных данных текущего пользователя.</summary>
public class AuthService
{
    /// <summary>Логин текущего пользователя (null, если не аутентифицирован).</summary>
    public string? Login { get; private set; }

    /// <summary>Пароль текущего пользователя (null, если не аутентифицирован).</summary>
    public string? Password { get; private set; }

    /// <summary>UUID текущего пользователя (null, если не аутентифицирован).</summary>
    public Guid? UserId { get; private set; }

    /// <summary>Признак того, что пользователь аутентифицирован.</summary>
    public bool IsAuthenticated => Login != null && Password != null;

    /// <summary>Сохраняет учётные данные после успешного логина.</summary>
    /// <param name="login">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="userId">UUID пользователя.</param>
    public void SetCredentials(string login, string password, Guid userId)
    {
        Login = login;
        Password = password;
        UserId = userId;
    }

    /// <summary>Очищает учётные данные (выход).</summary>
    public void Clear()
    {
        Login = null;
        Password = null;
        UserId = null;
    }

    /// <summary>Возвращает значение заголовка Authorization для Basic Auth.</summary>
    /// <returns>Строка вида "Basic base64(login:password)".</returns>
    public string GetAuthorizationHeader()
    {
        if (Login == null || Password == null) return string.Empty;
        var credentials = $"{Login}:{Password}";
        var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
        return $"Basic {encoded}";
    }
}