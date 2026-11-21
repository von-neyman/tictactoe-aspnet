namespace TicTacToe.Server.Web.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Domain.Service;
using TicTacToe.Server.Web.Mapper;
using TicTacToe.Server.Web.Model;

/// <summary>Контроллер регистрации и авторизации.</summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    /// <summary>Принимает сервис аутентификации через конструктор.</summary>
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    /// <summary>Регистрирует нового пользователя.</summary>
    /// <param name="request">Логин и пароль.</param>
    [HttpPost("signup")]
    [AllowAnonymous]
    public IActionResult SignUp([FromBody] Web.Model.SignUpRequest request)
    {
        var domainRequest = WebMapper.SignUpRequestToDomain(request);
        var success = authService.Register(domainRequest);
        if (!success) return Conflict(new ErrorResponse { Message = "Логин уже занят." });

        return Ok();
    }

    /// <summary>Авторизует пользователя по заголовку Authorization и возвращает его UUID.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        var header = Request.Headers["Authorization"].ToString();
        var userId = authService.Authenticate(header);
        if (userId == null) return Unauthorized(new ErrorResponse { Message = "Неверный логин или пароль." });

        return Ok(new { Id = userId.Value });
    }
}