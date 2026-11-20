namespace TicTacToe.Server.Datasource.Model;

using System.ComponentModel.DataAnnotations;

/// <summary>Модель пользователя для слоя данных.</summary>
public class UserEntity
{
    /// <summary>Уникальный идентификатор пользователя.</summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>Логин пользователя.</summary>
    [Required]
    [MaxLength(50)]
    public string Login { get; set; } = null!;

    /// <summary>Хеш пароля.</summary>
    [Required]
    public string PasswordHash { get; set; } = null!;
}