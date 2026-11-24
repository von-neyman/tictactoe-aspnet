namespace TicTacToe.Server.Datasource.Mapper;

using System.Text.Json;
using TicTacToe.Server.Datasource.Model;
using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;

/// <summary>Преобразование моделей между слоями Datasource и Domain.</summary>
internal static class DatasourceMapper
{
    /// <summary>Преобразует модель игры из слоя данных в доменную.</summary>
    internal static Game GameToDomain(GameEntity entity)
    {
        var field = GameFieldFromJson(entity.FieldJson);
        return new Game(
            entity.Id, entity.CreatorId, field,
            entity.PlayerXType, entity.PlayerXId,
            entity.PlayerOType, entity.PlayerOId,
            entity.Difficulty, entity.State,
            entity.CurrentTurnPlayerId, entity.WinnerPlayerId,
            entity.CreatedAt, entity.LastMoveAt, entity.MoveCount);
    }

    /// <summary>Преобразует доменную модель игры в модель слоя данных.</summary>
    internal static GameEntity GameToEntity(Game domain)
    {
        return new GameEntity
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            PlayerXType = domain.PlayerXType,
            PlayerXId = domain.PlayerXId,
            PlayerOType = domain.PlayerOType,
            PlayerOId = domain.PlayerOId,
            Difficulty = domain.Difficulty,
            State = domain.State,
            CurrentTurnPlayerId = domain.CurrentTurnPlayerId,
            WinnerPlayerId = domain.WinnerPlayerId,
            CreatedAt = domain.CreatedAt,
            LastMoveAt = domain.LastMoveAt,
            MoveCount = domain.MoveCount,
            FieldJson = GameFieldToJson(domain.Field)
        };
    }

    /// <summary>Сериализует поле в JSON.</summary>
    private static string GameFieldToJson(GameField field)
    {
        var cells = new int[GameField.Size][];
        for (int i = 0; i < GameField.Size; i++)
        {
            cells[i] = new int[GameField.Size];
            for (int j = 0; j < GameField.Size; j++)
                cells[i][j] = (int)field.Cells[i][j];
        }
        return JsonSerializer.Serialize(cells);
    }

    /// <summary>Десериализует поле из JSON.</summary>
    private static GameField GameFieldFromJson(string json)
    {
        var cells = JsonSerializer.Deserialize<int[][]>(json);
        var field = new GameField();
        for (int i = 0; i < GameField.Size; i++)
            for (int j = 0; j < GameField.Size; j++)
                field.Cells[i][j] = (CellState)cells![i][j];
        return field;
    }
}