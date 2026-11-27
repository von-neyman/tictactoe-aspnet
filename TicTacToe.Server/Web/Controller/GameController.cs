namespace TicTacToe.Server.Web.Controller;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Domain.Service;
using TicTacToe.Server.Web.Mapper;
using TicTacToe.Server.Web.Model;

/// <summary>Контроллер для игры в крестики-нолики.</summary>
[Authorize]
[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly IGameService gameService;
    private readonly IUserService userService;

    /// <summary>Принимает сервисы через конструктор.</summary>
    public GameController(IGameService gameService, IUserService userService)
    {
        this.gameService = gameService;
        this.userService = userService;
    }

    /// <summary>Создаёт новую игру и возвращает её.</summary>
    /// <param name="request">Параметры новой игры: символ и (опционально) сложность.</param>
    [HttpPost("new")]
    public IActionResult NewGame([FromBody] NewGameRequest request)
    {
        var userId = GetUserId();
        var game = gameService.NewGame(userId, request.PlayerSymbol, request.Difficulty);
        return Ok(WebMapper.GameToDto(game, userService));
    }

    /// <summary>Присоединяет текущего пользователя к игре.</summary>
    /// <param name="request">Идентификатор игры.</param>
    [HttpPost("join")]
    public IActionResult JoinGame([FromBody] JoinGameRequest request)
    {
        var userId = GetUserId();
        var success = gameService.JoinGame(request.GameId, userId);
        if (!success) return BadRequest(new ErrorResponse { Message = "Не удалось присоединиться к игре." });
        return Ok();
    }

    /// <summary>Возвращает список доступных игр (ожидание игроков).</summary>
    [HttpGet("available")]
    public IActionResult GetAvailableGames()
    {
        var userId = GetUserId();
        var games = gameService.GetAvailableGames(userId);
        var dtos = new List<AvailableGameDto>();
        foreach (var game in games)
        {
            var creator = userService.GetById(game.CreatorId);
            var login = creator?.Login ?? "Unknown";
            dtos.Add(WebMapper.GameToAvailableDto(game, login));
        }
        return Ok(dtos);
    }

    /// <summary>Возвращает список незавершённых игр текущего пользователя.</summary>
    [HttpGet("my")]
    public IActionResult GetUserGames()
    {
        var userId = GetUserId();
        var games = gameService.GetUserGames(userId);
        return Ok(games.Select(g => WebMapper.GameToDto(g, userService)).ToList());
    }

    /// <summary>Делает ход игрока и (если нужно) ход компьютера.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <param name="field">Состояние поля после хода.</param>
    [HttpPost("{id}/move")]
    public IActionResult MakeMove(Guid id, [FromBody] GameFieldDto field)
    {
        var userId = GetUserId();
        var playerField = WebMapper.GameFieldToDomain(field);
        var success = gameService.MakeMove(id, userId, playerField);
        if (!success) return BadRequest(new ErrorResponse { Message = "Некорректный ход." });
        return Ok();
    }

    /// <summary>Возвращает актуальное состояние игры.</summary>
    /// <param name="id">Идентификатор игры.</param>
    [HttpGet("{id}")]
    public IActionResult GetGame(Guid id)
    {
        var game = gameService.GetGame(id);
        if (game == null) return NotFound();
        return Ok(WebMapper.GameToDto(game, userService));
    }

    /// <summary>Возвращает UUID текущего пользователя из ClaimsPrincipal.</summary>
    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}