namespace TicTacToe.Server.DI;

using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Server.Datasource.Repository;
using TicTacToe.Server.Datasource.Service;
using TicTacToe.Server.Domain.Service;

/// <summary>Конфигурация графа зависимостей приложения.</summary>
public static class Configuration
{
    /// <summary>Регистрирует все зависимости в DI-контейнере.</summary>
    /// <param name="services">Коллекция сервисов.</param>
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton<GameStorage>();
        services.AddSingleton<IGameRepository, GameRepository>();
        services.AddSingleton<IGameService, GameService>();
    }
}