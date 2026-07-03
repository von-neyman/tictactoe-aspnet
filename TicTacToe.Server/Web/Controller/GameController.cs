namespace TicTacToe.Server.Web.Controller;

using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Datasource.Mapper;
using TicTacToe.Server.Datasource.Repository;
using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Service;
using TicTacToe.Server.Web.Mapper;
using TicTacToe.Server.Web.Model;

/// <summary>Контроллер для игры в крестики-нолики.</summary>
[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly IGameService gameService;
    private readonly IGameRepository repository;

    /// <summary>Принимает сервис и репозиторий через конструктор.</summary>
    public GameController(IGameService gameService, IGameRepository repository)
    {
        this.gameService = gameService;
        this.repository = repository;
    }

    /// <summary>Создаёт новую игру и возвращает её.</summary>
    /// <param name="playerSymbol">Символ, выбранный игроком (X или O).</param>
    [HttpPost("new")]
    public IActionResult NewGame([FromBody] CellState playerSymbol)
    {
        var game = new Game(playerSymbol);
        var gameEntity = DatasourceMapper.GameToEntity(game);
        repository.Save(gameEntity);
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
        var gameEntity = repository.GetById(id);
        if (gameEntity == null) return NotFound();
        var game = WebMapper.GameToDomain(dto);
        var isValid = gameService.ValidatePlayerMove(id, game.Field);
        if (!isValid) return BadRequest(new ErrorResponse { Message = "Некорректный ход. Попробуйте снова." });
        var updatedGameEntity = repository.GetById(id)!;
        var updatedGame = DatasourceMapper.GameToDomain(updatedGameEntity);
        if (updatedGame.State == GameState.InProgress) gameService.MakeComputerMove(id);
        var resultGameEntity = repository.GetById(id)!;
        var resultGame = DatasourceMapper.GameToDomain(resultGameEntity);
        var resultGameDto = WebMapper.GameToDto(resultGame);
        return Ok(resultGameDto);
    }

    /// <summary>Возвращает актуальное состояние игры.</summary>
    /// <param name="id">Идентификатор игры.</param>
    [HttpGet("{id}")]
    public IActionResult GetGame(Guid id)
    {
        var gameEntity = repository.GetById(id);
        if (gameEntity == null) return NotFound();
        var game = DatasourceMapper.GameToDomain(gameEntity);
        var gameDto = WebMapper.GameToDto(game);
        return Ok(gameDto);
    }
}