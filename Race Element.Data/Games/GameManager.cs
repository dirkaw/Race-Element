﻿using RaceElement.Core.Jobs.LoopJob;
using RaceElement.Data.Common;
using RaceElement.Data.Games.iRacing;

namespace RaceElement.Data.Games;

public static class GameManager
{
    private readonly static SimpleLoopJob _dataUpdaterJob = new()
    {
        Action = () => SimDataProvider.Update(),
        IntervalMillis = 1000 / 50        // TODO: adjust for each game.
    };

    public static Game CurrentGame { get; private set; } = Game.Any;


    public static event EventHandler<(Game previous, Game next)>? OnGameChanged;
    public static void SetCurrentGame(Game nextGame)
    {
        ExitGameData(CurrentGame);

        SimDataProvider.Clear();
        Game previousGame = CurrentGame;
        CurrentGame = nextGame;
        OnGameChanged?.Invoke(null, (previousGame, nextGame));

        _dataUpdaterJob.Run();
    }

    /// <summary>
    /// Gracefully disposes and stops all mechanisms that are required for a game so they do not interfere with other games.
    /// </summary>
    /// <param name="game"></param>
    private static void ExitGameData(Game game)
    {
        SimDataProvider.Stop();
        _dataUpdaterJob?.CancelJoin();
    }
}
