namespace TicTacToe.Client.Service;

/// <summary>Сервис для хранения статистики текущей сессии.</summary>
public class ScoreService
{
    /// <summary>Количество побед.</summary>
    public int Wins { get; private set; }

    /// <summary>Количество поражений.</summary>
    public int Losses { get; private set; }

    /// <summary>Количество ничьих.</summary>
    public int Draws { get; private set; }

    /// <summary>Увеличивает счётчик побед.</summary>
    public void AddWin() => Wins++;

    /// <summary>Увеличивает счётчик поражений.</summary>
    public void AddLoss() => Losses++;

    /// <summary>Увеличивает счётчик ничьих.</summary>
    public void AddDraw() => Draws++;

    /// <summary>Сбрасывает статистику.</summary>
    public void Reset()
    {
        Wins = 0;
        Losses = 0;
        Draws = 0;
    }
}