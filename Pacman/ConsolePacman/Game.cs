﻿using System;
using System.Threading;
using System.Threading.Tasks;

public class Game
{
    private readonly World _world;
    private readonly Pacman _pacman;
    private readonly Ghosts _ghosts;

    public Game(int ghostCount=4)  // 建構子
    {
        _world = new World();
        _pacman = new Pacman(_world);
        _ghosts = new Ghosts(_world, ghostCount);
    }

    public async Task<bool> RunAsync()
    {
        ShowWorldGetReady();
        if (!StartGame()) return false;

        var cts = new CancellationTokenSource();
        try
        {
            var inputTask = Task.Run(() => GetUserInput(cts), cts.Token);
            await PlayAsync(cts.Token);
            cts.Cancel();
        }
        catch (TaskCanceledException)
        {
        }

        return AskPlayAgain();
    }

    private void GetUserInput(CancellationTokenSource cts)
    {
        while (!cts.IsCancellationRequested)
        {
            var key = Console.ReadKey(true).Key;
            if (key is ConsoleKey.Escape)
            {
                cts.Cancel();
                return;
            }

            var direction = key switch
            {
                ConsoleKey.UpArrow => Direction.Up,
                ConsoleKey.DownArrow => Direction.Down,
                ConsoleKey.LeftArrow => Direction.Left,
                ConsoleKey.RightArrow => Direction.Right,
                _ => Direction.None
            };
            if (direction != Direction.None)
                AddDirection(direction);
        }
    }

    private void AddDirection(Direction direction)
        => _world.Directions.Enqueue(direction);

    private bool AskPlayAgain()
    {
        // TODO: AskPlayAgain
        return false;
    }

    private async Task PlayAsync(CancellationToken token)
    {
        var isGameOver = false;
        while (!isGameOver)
        {
            if (token.IsCancellationRequested)
                break;

            _pacman.PacmanMove();
            isGameOver = _world.UpdateScore(_pacman, _ghosts);

            if (!isGameOver)
            {
                _ghosts.GhostsMoveAndScore();
                isGameOver = _world.UpdateScore(_ghosts, _pacman);
            }

            await Task.Delay(100);  // 停 100 毫秒
        }

        _world.ShowGameOver();
    }

    private bool StartGame()
    {
        _world.ShowTextReady();
        Console.SetCursorPosition(0, 24);
        Console.WriteLine("Press [Enter] to play, [Escape] to quite.");
        
        var key = ConsoleKey.NoName;
        while (key != ConsoleKey.Enter && key != ConsoleKey.Escape) key = Console.ReadKey(true).Key;

        Console.SetCursorPosition(0, 24);
        Console.WriteLine();
        _world.EraseTextReady();
        
        return (key is ConsoleKey.Enter);
    }

    private void ShowWorldGetReady()
    {
        // TODO: ShowWorldGetReady
        _world.ShowWorld();
        _world.ShowDots();
        _world.ShowPacman(_pacman);
        _ghosts.Members.ForEach(ghost => _world.ShowGhost(ghost));
    }
}

