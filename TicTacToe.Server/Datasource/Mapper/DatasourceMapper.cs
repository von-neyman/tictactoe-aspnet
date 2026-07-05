namespace TicTacToe.Server.Datasource.Mapper;

using TicTacToe.Server.Datasource.Model;
using TicTacToe.Server.Domain.Model;

/// <summary>Преобразование моделей между слоями Datasource и Domain.</summary>
internal static class DatasourceMapper
{
    /// <summary>Преобразует модель поля из слоя данных в доменную.</summary>
    /// <param name="entity">Модель поля слоя данных.</param>
    /// <returns>Доменная модель поля.</returns>
    internal static GameField GameFieldToDomain(GameFieldEntity entity)
    {
        var field = new GameField();
        for (int i = 0; i < GameField.Size; i++)
            for (int j = 0; j < GameField.Size; j++)
                field.Cells[i][j] = entity.Cells[i][j];
        return field;
    }

    /// <summary>Преобразует доменную модель поля в модель слоя данных.</summary>
    /// <param name="domain">Доменная модель поля.</param>
    /// <returns>Модель поля слоя данных.</returns>
    internal static GameFieldEntity GameFieldToEntity(GameField domain)
    {
        var cells = new Domain.Enum.CellState[GameField.Size][];
        for (int i = 0; i < GameField.Size; i++)
        {
            cells[i] = new Domain.Enum.CellState[GameField.Size];
            for (int j = 0; j < GameField.Size; j++)
                cells[i][j] = domain.Cells[i][j];
        }
        return new GameFieldEntity { Cells = cells };
    }

    /// <summary>Преобразует модель игры из слоя данных в доменную.</summary>
    /// <param name="entity">Модель игры слоя данных.</param>
    /// <returns>Доменная модель игры.</returns>
    internal static Game GameToDomain(GameEntity entity)
    {
        return new Game(entity.PlayerSymbol, entity.Difficulty)
        {
            Id = entity.Id,
            Field = GameFieldToDomain(entity.Field),
            State = entity.State
        };
    }

    /// <summary>Преобразует доменную модель игры в модель слоя данных.</summary>
    /// <param name="domain">Доменная модель игры.</param>
    /// <returns>Модель игры слоя данных.</returns>
    internal static GameEntity GameToEntity(Game domain)
    {
        return new GameEntity
        {
            Id = domain.Id,
            Field = GameFieldToEntity(domain.Field),
            PlayerSymbol = domain.PlayerSymbol,
            ComputerSymbol = domain.ComputerSymbol,
            Difficulty = domain.Difficulty,
            State = domain.State
        };
    }
}