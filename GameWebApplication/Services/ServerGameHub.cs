using System.Collections.Concurrent;
using GameWebApplication.Models;
using Microsoft.AspNet.SignalR;

namespace GameWebApplication.Services
{
    public class ServerGameHub : Hub, IServerGameHub, IClientGameHub
    {
        private static readonly ConcurrentDictionary<string, IGame> Games = new ConcurrentDictionary<string, IGame>();

        public void CreateGame(CreateGameModel model)
        {
            var gameId = Context.ConnectionId;
            IGame game = new Game(gameId, model, this);
            Games.TryAdd(gameId, game);

            Clients.Caller.startGame(game);

            game.Start();
        }

        public void SetOccupied(int cellId)
        {
            var player = GetPlayer();
            player.SetOccupied(cellId);
        }

        public void Click(int cellId)
        {
            var player = GetPlayer();
            player.Click(cellId);
        }

        public void GameOver(Scores scores)
        {
            //Clients.Caller.gameOver(scores);
            RemoveGame();
        }

        #region Client
        public void ResetOccupiedCellClient(int cellId)
        {
            Clients.Caller.resetOccupied(cellId);
        }

        public void SetOccupiedCellClient(int cellId)
        {
            Clients.Caller.setOccupied(cellId);
        }

        public void ClickClient(int cellId)
        {
            Clients.Caller.resetOccupied(cellId);
            var game = GetGame();
            Clients.Caller.updateScore(game);
        }

        public void GameOverClient(Scores scores)
        {
            var game = GetGame();
            Clients.Caller.gameOver(game);
        }

        #endregion

        private IGamePlayer GetPlayer()
        {            
            var game = GetGame();
            return game.Player;
        }

        private IGame GetGame()
        {
            var gameId = Context.ConnectionId;
            IGame game;
            Games.TryGetValue(gameId, out game);

            return game;
        }

        private void RemoveGame()
        {
            IGame game;
            Games.TryRemove(Context.ConnectionId, out game);
        }
    }

    public interface IServerGameHub
    {
        void CreateGame(CreateGameModel model);
        void SetOccupied(int cellId);
        void Click(int cellId);
        void GameOver(Scores scores);
    }
}