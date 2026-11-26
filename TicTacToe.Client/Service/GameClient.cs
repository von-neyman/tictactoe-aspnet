namespace TicTacToe.Client.Service;

using System.Net.Http.Json;
using TicTacToe.Client.Enum;
using TicTacToe.Client.Model;

/// <summary>Клиент для взаимодействия с API игры.</summary>
public class GameClient
{
    private readonly HttpClient http;
    private readonly AuthService authService;

    /// <summary>Принимает HttpClient и AuthService через конструктор.</summary>
    public GameClient(HttpClient http, AuthService authService)
    {
        this.http = http;
        this.authService = authService;
    }

    /// <summary>Создаёт новую игру на сервере.</summary>
    /// <param name="playerSymbol">Символ игрока.</param>
    /// <param name="difficulty">Сложность (null — игра с человеком).</param>
    /// <returns>Созданная игра или null.</returns>
    public async Task<GameDto?> CreateGameAsync(CellState playerSymbol, int? difficulty)
    {
        var request = new NewGameRequest { PlayerSymbol = playerSymbol, Difficulty = difficulty };
        var response = await SendAsync(HttpMethod.Post, "game/new", request);
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameDto>();
    }

    /// <summary>Присоединяет текущего пользователя к игре.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>true, если присоединение успешно.</returns>
    public async Task<bool> JoinGameAsync(Guid gameId)
    {
        var request = new JoinGameRequest { GameId = gameId };
        var response = await SendAsync(HttpMethod.Post, "game/join", request);
        return response?.IsSuccessStatusCode ?? false;
    }

    /// <summary>Возвращает список доступных игр.</summary>
    /// <returns>Список игр или пустой список.</returns>
    public async Task<List<AvailableGameDto>> GetAvailableGamesAsync()
    {
        var response = await SendAsync(HttpMethod.Get, "game/available");
        if (response == null || !response.IsSuccessStatusCode) return new List<AvailableGameDto>();
        return await response.Content.ReadFromJsonAsync<List<AvailableGameDto>>() ?? new List<AvailableGameDto>();
    }

    /// <summary>Возвращает список незавершённых игр пользователя.</summary>
    /// <returns>Список игр или пустой список.</returns>
    public async Task<List<GameDto>> GetUserGamesAsync()
    {
        var response = await SendAsync(HttpMethod.Get, "game/my");
        if (response == null || !response.IsSuccessStatusCode) return new List<GameDto>();
        return await response.Content.ReadFromJsonAsync<List<GameDto>>() ?? new List<GameDto>();
    }

    /// <summary>Делает ход в игре.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="field">Поле после хода.</param>
    /// <returns>true, если ход успешен.</returns>
    public async Task<bool> MakeMoveAsync(Guid gameId, GameFieldDto field)
    {
        var response = await SendAsync(HttpMethod.Post, $"game/{gameId}/move", field);
        return response?.IsSuccessStatusCode ?? false;
    }

    /// <summary>Возвращает актуальное состояние игры.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Игра или null.</returns>
    public async Task<GameDto?> GetGameAsync(Guid gameId)
    {
        var response = await SendAsync(HttpMethod.Get, $"game/{gameId}");
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameDto>();
    }

    /// <summary>Отправляет HTTP-запрос с заголовком Authorization.</summary>
    private async Task<HttpResponseMessage?> SendAsync(HttpMethod method, string url, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        var authHeader = authService.GetAuthorizationHeader();
        if (!string.IsNullOrEmpty(authHeader)) request.Headers.Add("Authorization", authHeader);

        if (body != null) request.Content = JsonContent.Create(body);

        return await http.SendAsync(request);
    }
}