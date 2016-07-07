using System;
using System.Collections.Concurrent;
using GameWebApplication.Hubs;
using GameWebApplication.Models;

namespace GameWebApplication.Tests
{
    public class GameTestable: Game
    {
        public new ConcurrentDictionary<int, Cell> Cells => base.Cells;
        public new ConcurrentQueue<Action> Events => base.Events;

        public GameTestable(string gameId, CreateGameModel model, IClientGameHub playerEvents) : base(gameId, model, playerEvents)
        {

        }
    }
}
