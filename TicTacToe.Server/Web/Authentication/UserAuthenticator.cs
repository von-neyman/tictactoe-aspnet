namespace TicTacToe.Server.Web.Authentication;

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TicTacToe.Server.Domain.Service;

/// <summary>Кастомный обработчик Basic Auth для аутентификации пользователей.</summary>
public class UserAuthenticator : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthService authService;

    /// <summary>Конструктор обработчика.</summary>
    public UserAuthenticator(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IAuthService authService)
        : base(options, logger, encoder)
    {
        this.authService = authService;
    }

    /// <summary>Обрабатывает аутентификацию по заголовку Authorization: Basic base64(login:password).</summary>
    /// <returns>Результат аутентификации.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization")) return Task.FromResult(AuthenticateResult.NoResult());

        var header = Request.Headers["Authorization"].ToString();
        var userId = authService.Authenticate(header);

        if (userId == null) return Task.FromResult(AuthenticateResult.Fail("Invalid login or password."));

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}