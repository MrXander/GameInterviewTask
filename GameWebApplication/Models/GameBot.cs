using System;
using System.Diagnostics;
using System.Threading;
using GameWebApplication.Hubs;

namespace GameWebApplication.Models
{
    internal class GameBot : IGameBot
    {
        private const int MIN_DELAY_MS = 500;
        private const int MAX_DELAY_MS = 2000;

        private readonly int _cellsCount;
        private readonly IGameClient _game;
        private readonly Random _random;
        private readonly Timer _timer;

        public GameBot(int id, string name, int cellsCount, IGameClient game)
        {
            Id = id;
            Name = name;
            _cellsCount = cellsCount;
            _game = game;
            _random = new Random();
            _timer = new Timer(MakeTurn);
        }

        public int Id { get; }
        public string Name { get; }

        public void Start()
        {
            _timer.Change(0, 0);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void SetOccupied(int cellId)
        {
            _game.SetOccupiedCell(cellId, this);
        }

        public void Click(int cellId)
        {
            _game.Click(this, cellId);
        }

        private void MakeTurn(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            var timeout = GetTimeout();
            Debug.WriteLine(timeout);
            Thread.Sleep(timeout);

            //occupying cell
            var cellId = _random.Next(0, _cellsCount);
            SetOccupied(cellId);

            timeout = GetTimeout();
            Debug.WriteLine(timeout);
            Thread.Sleep(timeout);
            Click(cellId);

            Start();
        }

        private int GetTimeout()
        {
            return _random.Next(MIN_DELAY_MS, MAX_DELAY_MS);
        }
    }
}