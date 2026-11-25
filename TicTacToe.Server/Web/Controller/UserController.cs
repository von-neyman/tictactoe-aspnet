namespace TicTacToe.Server.Web.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Domain.Service;
using TicTacToe.Server.Web.Model;

/// <summary>Контроллер для получения информации о пользователе.</summary>
[Authorize]
[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    /// <summary>Принимает сервис пользователей через конструктор.</summary>
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>Возвращает информацию о пользователе по UUID.</summary>
    /// <param name="id">UUID пользователя.</param>
    [HttpGet("{id}")]
    public IActionResult GetUser(Guid id)
    {
        var user = userService.GetById(id);
        if (user == null) return NotFound();

        var dto = new UserDto { Id = user.Id, Login = user.Login };
        return Ok(dto);
    }
}