namespace GameWebApplication.Services
{
    public interface IGameClient
    {
        void SetOccupiedCell(int cellId, IGamePlayer player);
        void ResetOccupiedCell(int cellId, IGamePlayer player);
        void Click(IGamePlayer player, int cellId);        
    }
}