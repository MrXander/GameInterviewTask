using GameWebApplication.Hubs;

namespace GameWebApplication.Models
{
    internal class Player : IGamePlayer
    {
        private readonly IGameClient _game;

        public Player(int id, string name, IGameClient game)
        {
            Id = id;
            Name = name;
            _game = game;
        }

        public int Id { get; }
        public string Name { get; }

        public void SetOccupied(int cellId)
        {
            _game.SetOccupiedCell(cellId, this);
        }

        public void Click(int cellId)
        {
            _game.Click(this, cellId);
        }
    }
}