namespace TicTacToe.Client.Service;

using System.Net.Http.Json;
using TicTacToe.Client.Enum;
using TicTacToe.Client.Model;

/// <summary>Клиент для взаимодействия с API игры.</summary>
public class GameClient
{
    private readonly HttpClient http;

    /// <summary>Принимает HttpClient через конструктор.</summary>
    public GameClient(HttpClient http)
    {
        this.http = http;
    }

    /// <summary>Создаёт новую игру на сервере.</summary>
    /// <param name="playerSymbol">Символ игрока.</param>
    /// <param name="difficulty">Сложность.</param>
    /// <returns>Созданная игра.</returns>
    public async Task<GameDto?> CreateGame(CellState playerSymbol, int difficulty)
    {
        var request = new NewGameRequest { PlayerSymbol = playerSymbol, Difficulty = difficulty };
        var response = await http.PostAsJsonAsync("game/new", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameDto>();
    }

    /// <summary>Отправляет ход игрока на сервер.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <param name="game">Игра с ходом игрока.</param>
    /// <returns>Обновлённая игра или null при ошибке.</returns>
    public async Task<GameDto?> MakeMove(Guid gameId, GameDto game)
    {
        var response = await http.PostAsJsonAsync($"game/{gameId}", game);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameDto>();
    }

    /// <summary>Получает актуальное состояние игры.</summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Игра или null.</returns>
    public async Task<GameDto?> GetGame(Guid gameId)
    {
        var response = await http.GetAsync($"game/{gameId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GameDto>();
    }
}