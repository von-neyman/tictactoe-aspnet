namespace TicTacToe.Server.Web.Mapper;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Web.Model;

/// <summary>Преобразование моделей между слоями Web и Domain.</summary>
internal static class WebMapper
{
    /// <summary>Преобразует модель поля из веб-слоя в доменную.</summary>
    /// <param name="dto">Модель поля от клиента.</param>
    /// <returns>Доменная модель поля.</returns>
    internal static GameField GameFieldToDomain(GameFieldDto dto)
    {
        var field = new GameField();
        for (int i = 0; i < GameField.Size; i++)
            for (int j = 0; j < GameField.Size; j++)
                field.Cells[i][j] = dto.Cells[i][j];
        return field;
    }

    /// <summary>Преобразует доменную модель поля в модель веб-слоя.</summary>
    /// <param name="domain">Доменная модель поля.</param>
    /// <returns>Модель поля для клиента.</returns>
    internal static GameFieldDto GameFieldToDto(GameField domain)
    {
        var cells = new CellState[GameField.Size][];
        for (int i = 0; i < GameField.Size; i++)
        {
            cells[i] = new CellState[GameField.Size];
            for (int j = 0; j < GameField.Size; j++)
                cells[i][j] = domain.Cells[i][j];
        }
        return new GameFieldDto { Cells = cells };
    }

    /// <summary>Преобразует модель игры из веб-слоя в доменную.</summary>
    /// <param name="dto">Модель игры от клиента.</param>
    /// <returns>Доменная модель игры.</returns>
    internal static Game GameToDomain(GameDto dto)
    {
        return new Game(dto.PlayerSymbol, dto.Difficulty)
        {
            Id = dto.Id,
            Field = GameFieldToDomain(dto.Field),
            State = dto.State
        };
    }

    /// <summary>Преобразует доменную модель игры в модель веб-слоя.</summary>
    /// <param name="domain">Доменная модель игры.</param>
    /// <returns>Модель игры для клиента.</returns>
    internal static GameDto GameToDto(Game domain)
    {
        return new GameDto
        {
            Id = domain.Id,
            Field = GameFieldToDto(domain.Field),
            PlayerSymbol = domain.PlayerSymbol,
            ComputerSymbol = domain.ComputerSymbol,
            Difficulty = domain.Difficulty,
            State = domain.State
        };
    }
}