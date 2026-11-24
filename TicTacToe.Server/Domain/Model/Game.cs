namespace TicTacToe.Server.Domain.Model;

using TicTacToe.Server.Domain.Enum;

/// <summary>Текущая игра в крестики-нолики.</summary>
public record Game
{
    /// <summary>Уникальный идентификатор игры.</summary>
    public Guid Id { get; init; }

    /// <summary>UUID создателя игры.</summary>
    public Guid CreatorId { get; init; }

    /// <summary>Текущее состояние игрового поля.</summary>
    public GameField Field { get; init; }

    /// <summary>Тип игрока в слоте X.</summary>
    public PlayerType PlayerXType { get; init; }

    /// <summary>UUID игрока в слоте X (null для компьютера или пустого слота).</summary>
    public Guid? PlayerXId { get; init; }

    /// <summary>Тип игрока в слоте O.</summary>
    public PlayerType PlayerOType { get; init; }

    /// <summary>UUID игрока в слоте O (null для компьютера или пустого слота).</summary>
    public Guid? PlayerOId { get; init; }

    /// <summary>
    /// Сложность игры: максимальная глубина просчёта ходов компьютером в полуходах (depth).
    /// 0 — компьютер не просчитывает ходы вообще, выбирает случайную пустую клетку.
    /// 1 — компьютер видит только свои ближайшие ходы (может завершить победу, но не видит угроз).
    /// 2 — видит свои ближайшие ходы и ответ игрока (может защититься от немедленного поражения).
    /// Size * Size — просчитывает всё, идеальная игра.
    /// null — игра между двумя людьми.
    /// </summary>
    public int? Difficulty { get; init; }

    /// <summary>Текущее состояние игры.</summary>
    public GameState State { get; init; }

    /// <summary>UUID игрока, чей сейчас ход (для состояния PlayerTurn).</summary>
    public Guid? CurrentTurnPlayerId { get; init; }

    /// <summary>UUID победителя (для состояния PlayerWon).</summary>
    public Guid? WinnerPlayerId { get; init; }

    /// <summary>Дата и время создания игры.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>Дата и время последнего хода.</summary>
    public DateTime LastMoveAt { get; init; }

    /// <summary>Количество сделанных ходов.</summary>
    public int MoveCount { get; init; }

    /// <summary>
    /// Создаёт новую игру.
    /// </summary>
    /// <param name="creatorId">UUID создателя игры (человека).</param>
    /// <param name="creatorSymbol">Символ создателя (X или O).</param>
    /// <param name="difficulty">Сложность игры с компьютером (null — игра с человеком).</param>
    public Game(Guid creatorId, CellState creatorSymbol, int? difficulty = null)
    {
        Id = Guid.NewGuid();
        CreatorId = creatorId;
        Field = new GameField();
        Difficulty = difficulty;
        CreatedAt = DateTime.UtcNow;
        LastMoveAt = DateTime.UtcNow;
        MoveCount = 0;
        WinnerPlayerId = null;

        var isComputerGame = difficulty.HasValue;

        if (creatorSymbol == CellState.X)
        {
            PlayerXType = PlayerType.Human;
            PlayerXId = creatorId;
            PlayerOType = isComputerGame ? PlayerType.Computer : PlayerType.None;
            PlayerOId = null;
            CurrentTurnPlayerId = creatorId;
        }
        else
        {
            PlayerXType = isComputerGame ? PlayerType.Computer : PlayerType.None;
            PlayerXId = null;
            PlayerOType = PlayerType.Human;
            PlayerOId = creatorId;
            CurrentTurnPlayerId = null;
        }

        State = isComputerGame ? GameState.PlayerTurn : GameState.WaitingForPlayers;
    }

    /// <summary>Присоединяет второго игрока к игре.</summary>
    /// <param name="playerId">UUID присоединяющегося игрока.</param>
    /// <returns>Новая игра с заполненным вторым слотом.</returns>
    public Game JoinAsOpponent(Guid playerId)
    {
        if (State != GameState.WaitingForPlayers) throw new InvalidOperationException("Игра уже начата.");
        if (PlayerXId == playerId || PlayerOId == playerId) throw new InvalidOperationException("Вы уже в этой игре.");

        if (PlayerXType == PlayerType.None)
        {
            // Создатель выбрал O. Присоединяющийся играет за X. Создатель (O) не мог ходить, поэтому CurrentTurnPlayerId был null. Теперь ход X, значит playerId.
            return this with
            {
                PlayerXType = PlayerType.Human,
                PlayerXId = playerId,
                State = GameState.PlayerTurn,
                CurrentTurnPlayerId = playerId
            };
        }
        else
        {
            // Создатель выбрал X. Присоединяющийся играет за O. CurrentTurnPlayerId может быть:
            // - creatorId (создатель ещё не ходил) — тогда ход остаётся за X.
            // - null (создатель уже ходил) — тогда ход переходит к O (playerId).
            return this with
            {
                PlayerOType = PlayerType.Human,
                PlayerOId = playerId,
                State = GameState.PlayerTurn,
                CurrentTurnPlayerId = CurrentTurnPlayerId ?? playerId
            };
        }
    }

    /// <summary>Создаёт экземпляр игры с новым полем и указанным состоянием.</summary>
    /// <param name="newField">Новое состояние игрового поля.</param>
    /// <param name="newState">Новое состояние игры.</param>
    /// <param name="nextTurnPlayerId">UUID игрока, чей следующий ход (или null).</param>
    /// <param name="winnerPlayerId">UUID победителя (или null).</param>
    /// <returns>Новая игра с обновлённым полем и состоянием.</returns>
    public Game UpdateField(GameField newField, GameState newState, Guid? nextTurnPlayerId, Guid? winnerPlayerId)
    {
        return this with
        {
            Field = newField,
            State = newState,
            CurrentTurnPlayerId = nextTurnPlayerId,
            WinnerPlayerId = winnerPlayerId,
            MoveCount = MoveCount + 1,
            LastMoveAt = DateTime.UtcNow
        };
    }

    /// <summary>Создаёт игру из сохранённого состояния (для маппинга из БД).</summary>
    internal Game(
        Guid restoredId, Guid restoredCreatorId, GameField restoredField,
        PlayerType restoredPlayerXType, Guid? restoredPlayerXId,
        PlayerType restoredPlayerOType, Guid? restoredPlayerOId,
        int? restoredDifficulty, GameState restoredState,
        Guid? restoredCurrentTurnPlayerId, Guid? restoredWinnerPlayerId,
        DateTime restoredCreatedAt, DateTime restoredLastMoveAt, int restoredMoveCount)
    {
        Id = restoredId;
        CreatorId = restoredCreatorId;
        Field = restoredField;
        PlayerXType = restoredPlayerXType;
        PlayerXId = restoredPlayerXId;
        PlayerOType = restoredPlayerOType;
        PlayerOId = restoredPlayerOId;
        Difficulty = restoredDifficulty;
        State = restoredState;
        CurrentTurnPlayerId = restoredCurrentTurnPlayerId;
        WinnerPlayerId = restoredWinnerPlayerId;
        CreatedAt = restoredCreatedAt;
        LastMoveAt = restoredLastMoveAt;
        MoveCount = restoredMoveCount;
    }
}