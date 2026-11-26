namespace TicTacToe.Client.Service;

using System.Net.Http.Json;
using TicTacToe.Client.Model;

/// <summary>Клиент для взаимодействия с API аутентификации.</summary>
public class AuthClient
{
    private readonly HttpClient http;

    /// <summary>Принимает HttpClient через конструктор.</summary>
    public AuthClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="login">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>true, если регистрация успешна.</returns>
    public async Task<bool> SignUpAsync(string login, string password)
    {
        var request = new SignUpRequest { Login = login, Password = password };
        var response = await http.PostAsJsonAsync("auth/signup", request);
        return response.IsSuccessStatusCode;
    }

    /// <summary>Авторизует пользователя и возвращает UUID.</summary>
    /// <param name="login">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>UUID пользователя или null, если авторизация не удалась.</returns>
    public async Task<Guid?> LoginAsync(string login, string password)
    {
        var credentials = $"{login}:{password}";
        var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/login");
        request.Headers.Add("Authorization", $"Basic {encoded}");

        var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result?.Id;
    }

    /// <summary>Внутренняя модель ответа логина.</summary>
    private class LoginResponse
    {
        public Guid Id { get; set; }
    }
}