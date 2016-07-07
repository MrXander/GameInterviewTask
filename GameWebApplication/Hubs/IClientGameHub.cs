using GameWebApplication.Models;

namespace GameWebApplication.Hubs
{
    public interface IClientGameHub
    {
        void ResetOccupiedCellClient(int cellId);
        void SetOccupiedCellClient(int cellId);
        void ClickClient(int cellId);
        void GameOverClient(Scores scores);
    }
}