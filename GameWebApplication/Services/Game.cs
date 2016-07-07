using System;
using System.Collections.Concurrent;
using System.Threading;
using GameWebApplication.Models;

namespace GameWebApplication.Services
{
    public class Game : IGame, IGameClient
    {
        private const int CHECK_EVENTS_INTERVAL_MS = 60 / 30 * 100;
        private readonly ConcurrentDictionary<int, Cell> _cells;
        private readonly ConcurrentQueue<Action> _events;
        private readonly Timer _eventsTimer;
        private readonly IClientGameHub _playerEvents;
        private readonly ConcurrentDictionary<string, int> _score;
        private DateTime _gameEnded;
        private DateTime _gameStarted;
        private object _lockObj = new object();

        public Game(string gameId, CreateGameModel model, IClientGameHub playerEvents)
        {
            _playerEvents = playerEvents;
            _events = new ConcurrentQueue<Action>();
            _cells = new ConcurrentDictionary<int, Cell>();
            _score = new ConcurrentDictionary<string, int>();
            _eventsTimer = new Timer(RunEvents);

            PlayerName = model.PlayerName;
            PlayersCount = model.PlayersCount;
            Scores = new Scores();

            InitializePlayers();
            InitializeCells();
            InitializeScores();
        }

        private void InitializeScores()
        {
            foreach (var player in Players)
            {
                Scores.ScoreList.Add(new Score { Name = player.Name });
            }
        }

        private void InitializeCells()
        {
            for (var i = 0; i < CellsCount; i++)
            {
                var c = new Cell { Id = i };
                _cells.AddOrUpdate(i, c, (idx, oldCell) => c);
            }
        }

        private string PlayerName { get; }
        public int PlayersCount { get; }

        public int CellsCount => PlayersCount - 1;

        private int MaxScore => PlayersCount * 3;

        private TimeSpan Duration => _gameEnded.Subtract(_gameStarted);

        private IGameBot[] Bots { get; set; }
        private IGamePlayer[] Players { get; set; }
        public Scores Scores { get; private set; }
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
            _events.Enqueue(() =>
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
            _events.Enqueue(() =>
            {
                Cell cell;
                _cells.TryGetValue(cellId, out cell);

                if (!cell.IsOccupied) return;

                _score.AddOrUpdate(player.Name, 1, (key, oldValue) =>
                {
                    var newValue = oldValue + 1;
                    if (newValue == MaxScore)
                    {
                        GameOver();
                    }
                    return newValue;
                });

                UpdateScores();

                DispatchClientEvent(player.Id, cellId, _playerEvents.ClickClient);
            });
        }

        //TODO: refactor this shit
        private void UpdateScores()
        {
            foreach (var score in Scores.ScoreList)
            {
                int points = 0;
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
            if (_events.TryDequeue(out e))
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
            var c = new Cell { Id = cellId };
            _cells.AddOrUpdate(cellId, c, (k, v) => c);
        }
        private void OccupyCell(int cellId, IGamePlayer player)
        {
            var c = new Cell
            {
                Id = cellId,
                OccupiedBy = player
            };
            _cells.AddOrUpdate(cellId, c, (k, v) => c);

            DispatchClientEvent(player.Id, cellId, _playerEvents.SetOccupiedCellClient);
        }
    }
}