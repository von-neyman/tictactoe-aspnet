namespace TicTacToe.Server.Web.Controller;

using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Service;
using TicTacToe.Server.Web.Mapper;
using TicTacToe.Server.Web.Model;

/// <summary>Контроллер для игры в крестики-нолики.</summary>
[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly IGameService gameService;

    /// <summary>Принимает сервис через конструктор.</summary>
    public GameController(IGameService gameService)
    {
        this.gameService = gameService;
    }

    /// <summary>Создаёт новую игру и возвращает её.</summary>
    /// <param name="playerSymbol">Символ, выбранный игроком (X или O).</param>
    [HttpPost("new")]
    public IActionResult NewGame([FromBody] CellState playerSymbol)
    {
        var game = gameService.NewGame(playerSymbol);
        var gameDto = WebMapper.GameToDto(game);
        return Ok(gameDto);
    }

    /// <summary>Обрабатывает ход игрока и возвращает обновлённую игру с ходом компьютера.</summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <param name="dto">Модель игры с ходом игрока.</param>
    /// <returns>Обновлённая игра или ошибка.</returns>
    [HttpPost("{id}")]
    public IActionResult MakeMove(Guid id, [FromBody] GameDto dto)
    {
        var game = gameService.GetGame(id);
        if (game == null) return NotFound();
        var playerGame = WebMapper.GameToDomain(dto);
        var isValid = gameService.ValidatePlayerMove(id, playerGame.Field);
        if (!isValid) return BadRequest(new ErrorResponse { Message = "Некорректный ход. Попробуйте снова." });
        var updatedGame = gameService.GetGame(id)!;
        if (updatedGame.State == GameState.InProgress) gameService.MakeComputerMove(id);
        var resultGame = gameService.GetGame(id)!;
        var resultGameDto = WebMapper.GameToDto(resultGame);
        return Ok(resultGameDto);
    }

    /// <summary>Возвращает актуальное состояние игры.</summary>
    /// <param name="id">Идентификатор игры.</param>
    [HttpGet("{id}")]
    public IActionResult GetGame(Guid id)
    {
        var game = gameService.GetGame(id);
        if (game == null) return NotFound();
        var gameDto = WebMapper.GameToDto(game);
        return Ok(gameDto);
    }
}