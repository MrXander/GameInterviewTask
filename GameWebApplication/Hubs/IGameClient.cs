using GameWebApplication.Models;

namespace GameWebApplication.Hubs
{
    public interface IGameClient
    {
        void SetOccupiedCell(int cellId, IGamePlayer player);
        void ResetOccupiedCell(int cellId, IGamePlayer player);
        void Click(IGamePlayer player, int cellId);
    }
}