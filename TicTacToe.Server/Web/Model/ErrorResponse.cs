namespace TicTacToe.Server.Web.Model;

/// <summary>Модель ошибки для ответа клиенту.</summary>
public class ErrorResponse
{
    /// <summary>Сообщение об ошибке.</summary>
    public string Message { get; set; } = null!;
}