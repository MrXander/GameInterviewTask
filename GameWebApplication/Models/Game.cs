using System;
using System.Collections.Concurrent;
using System.Threading;
using GameWebApplication.Hubs;

namespace GameWebApplication.Models
{
    public class Game : IGame, IGameClient
    {
        private const int CHECK_EVENTS_INTERVAL_MS = 60/30*100;
        private readonly Timer _eventsTimer;
        private readonly object _lockObj = new object();
        private readonly IClientGameHub _playerEvents;
        private readonly ConcurrentDictionary<string, int> _score;
        public readonly ConcurrentDictionary<int, Cell> Cells;
        public readonly ConcurrentQueue<Action> Events;
        private DateTime _gameEnded;
        private DateTime _gameStarted;

        public Game(string gameId, CreateGameModel model, IClientGameHub playerEvents)
        {
            if (model.PlayersCount <= 1) throw new ArgumentException("Players should be more than 1.");

            _playerEvents = playerEvents;
            Events = new ConcurrentQueue<Action>();
            Cells = new ConcurrentDictionary<int, Cell>();
            _score = new ConcurrentDictionary<string, int>();
            _eventsTimer = new Timer(RunEvents);

            PlayerName = model.PlayerName;
            PlayersCount = model.PlayersCount;
            Scores = new Scores();

            InitializePlayers();
            InitializeCells();
            InitializeScores();
        }

        private string PlayerName { get; }

        private int MaxScore => PlayersCount*3;

        private TimeSpan Duration => _gameEnded.Subtract(_gameStarted);

        private IGameBot[] Bots { get; set; }
        private IGamePlayer[] Players { get; set; }
        public int PlayersCount { get; }

        public int CellsCount => PlayersCount - 1;
        public Scores Scores { get; }
        public IGamePlayer Player { get; private set; }

        public void Start()
        {
            foreach (var bot in Bots)
            {
                bot.Start();
            }
            _gameStarted = DateTime.UtcNow;
            _eventsTimer.Change(CHECK_EVENTS_INTERVAL_MS, CHECK_EVENTS_INTERVAL_MS);
        }

        public void Stop()
        {
            foreach (var bot in Bots)
            {
                bot.Stop();
            }
            _gameEnded = DateTime.UtcNow;
            _eventsTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _eventsTimer.Dispose();
        }

        public void SetOccupiedCell(int cellId, IGamePlayer player)
        {
            Events.Enqueue(() =>
            {
                ResetOccupiedCell(cellId, player);
                OccupyCell(cellId, player);
            });
        }

        public void ResetOccupiedCell(int cellId, IGamePlayer player)
        {
            ResetCell(cellId, player);
            DispatchClientEvent(player.Id, cellId, _playerEvents.ResetOccupiedCellClient);
        }

        public void Click(IGamePlayer player, int cellId)
        {
            Events.Enqueue(() =>
            {
                Cell cell;
                Cells.TryGetValue(cellId, out cell);

                if (!cell.IsOccupied && cell.OccupiedBy.Id != player.Id) return;

                _score.AddOrUpdate(player.Name, 1, (key, oldValue) =>
                {
                    var newValue = oldValue + 1;
                    if (newValue == MaxScore)
                    {
                        GameOver();
                    }
                    return newValue;
                });

                ResetCell(cellId, player);

                UpdateScores();

                DispatchClientEvent(player.Id, cellId, _playerEvents.ClickClient);
            });
        }

        private void InitializeScores()
        {
            foreach (var player in Players)
            {
                Scores.ScoreList.Add(new Score {Name = player.Name});
            }
        }

        private void InitializeCells()
        {
            for (var i = 0; i < CellsCount; i++)
            {
                var c = new Cell {Id = i};
                Cells.AddOrUpdate(i, c, (idx, oldCell) => c);
            }
        }

        //TODO: refactor this shit
        private void UpdateScores()
        {
            foreach (var score in Scores.ScoreList)
            {
                var points = 0;
                _score.TryGetValue(score.Name, out points);
                lock (_lockObj)
                {
                    score.Points = points;
                }
            }
        }

        private void DispatchClientEvent(int playerId, int cellId, Action<int> clientEvent)
        {
            if (playerId == Player.Id) return;

            clientEvent(cellId);
        }

        public void GameOver()
        {
            Stop();
            _playerEvents.GameOverClient(Scores);
        }

        private void InitializePlayers()
        {
            CreateBots();
            Players = new IGamePlayer[PlayersCount];
            Player = Players[0] = new Player(0, PlayerName, this);
            for (var i = 0; i < CellsCount; i++)
            {
                Players[i + 1] = Bots[i];
            }
        }

        private void RunEvents(object state)
        {
            Action e;
            if (Events.TryDequeue(out e))
            {
                e();
            }
        }

        private void CreateBots()
        {
            Bots = new IGameBot[CellsCount];
            for (var i = 0; i < CellsCount; i++)
            {
                Bots[i] = new GameBot(i + 1, $"Bot {i}", CellsCount, this);
            }
        }

        private void ResetCell(int cellId, IGamePlayer player)
        {
            var c = new Cell {Id = cellId};
            Cells.AddOrUpdate(cellId, c, (k, v) => c);
        }

        private void OccupyCell(int cellId, IGamePlayer player)
        {
            var c = new Cell
            {
                Id = cellId,
                OccupiedBy = player
            };
            Cells.AddOrUpdate(cellId, c, (k, v) => c);

            DispatchClientEvent(player.Id, cellId, _playerEvents.SetOccupiedCellClient);
        }
    }
}