namespace TicTacToe.Server.DI;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TicTacToe.Server.Datasource;
using TicTacToe.Server.Datasource.Repository;
using TicTacToe.Server.Datasource.Service;
using TicTacToe.Server.Domain.Repository;
using TicTacToe.Server.Domain.Service;

/// <summary>Конфигурация графа зависимостей приложения.</summary>
public static class Configuration
{
    /// <summary>Регистрирует все зависимости в DI-контейнере.</summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var password = configuration["DbPassword"];
        if (!string.IsNullOrEmpty(password))
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Password = password
            };
            connectionString = builder.ConnectionString;
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddSingleton<GameStorage>();
        services.AddSingleton<IGameRepository, GameRepository>();
        services.AddSingleton<IGameService, GameService>();
    }
}