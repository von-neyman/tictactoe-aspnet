namespace TicTacToe.Server.Web.Mapper;

using TicTacToe.Server.Domain.Enum;
using TicTacToe.Server.Domain.Model;
using TicTacToe.Server.Domain.Service;
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

    /// <summary>Преобразует доменную модель игры в модель веб-слоя.</summary>
    /// <param name="domain">Доменная модель игры.</param>
    /// <param name="userService">Сервис для работы с пользователями.</param>
    /// <returns>Модель игры для клиента.</returns>
    internal static GameDto GameToDto(Game domain, IUserService userService)
    {
        return new GameDto
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            Field = GameFieldToDto(domain.Field),
            PlayerXType = domain.PlayerXType,
            PlayerXId = domain.PlayerXId,
            PlayerXLogin = domain.PlayerXId != null ? userService.GetById(domain.PlayerXId.Value)?.Login : null,
            PlayerOType = domain.PlayerOType,
            PlayerOId = domain.PlayerOId,
            PlayerOLogin = domain.PlayerOId != null ? userService.GetById(domain.PlayerOId.Value)?.Login : null,
            Difficulty = domain.Difficulty,
            State = domain.State,
            CurrentTurnPlayerId = domain.CurrentTurnPlayerId,
            WinnerPlayerId = domain.WinnerPlayerId,
            CreatedAt = domain.CreatedAt,
            LastMoveAt = domain.LastMoveAt,
            MoveCount = domain.MoveCount
        };
    }

    /// <summary>Преобразует доменную модель игры в модель доступной игры для списка.</summary>
    /// <param name="domain">Доменная модель игры.</param>
    /// <param name="creatorLogin">Логин создателя игры.</param>
    /// <returns>Модель доступной игры.</returns>
    internal static AvailableGameDto GameToAvailableDto(Game domain, string creatorLogin)
    {
        var creatorSymbol = domain.PlayerXId == domain.CreatorId ? CellState.X : CellState.O;
        return new AvailableGameDto
        {
            Id = domain.Id,
            CreatorLogin = creatorLogin,
            CreatorSymbol = creatorSymbol,
            CreatedAt = domain.CreatedAt
        };
    }

    /// <summary>Преобразует Web-модель SignUpRequest в доменную.</summary>
    /// <param name="request">Модель запроса на регистрацию от клиента.</param>
    /// <returns>Доменная модель запроса на регистрацию.</returns>
    internal static Domain.Model.SignUpRequest SignUpRequestToDomain(Web.Model.SignUpRequest request)
    {
        return new Domain.Model.SignUpRequest
        {
            Login = request.Login,
            Password = request.Password
        };
    }
}