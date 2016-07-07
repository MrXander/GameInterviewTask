using GameWebApplication.Models;

namespace GameWebApplication.Hubs
{
    public interface IServerGameHub
    {
        void CreateGame(CreateGameModel model);
        void SetOccupied(int cellId);
        void Click(int cellId);
        void GameOver(Scores scores);
    }
}