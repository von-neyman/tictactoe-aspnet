namespace TicTacToe.Server.Datasource;

using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Datasource.Model;

/// <summary>Контекст базы данных приложения.</summary>
public class AppDbContext : DbContext
{
    /// <summary>Конструктор контекста.</summary>
    /// <param name="options">Опции контекста.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Пользователи.</summary>
    public DbSet<UserEntity> Users { get; set; } = null!;
}