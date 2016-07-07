namespace GameWebApplication.Models
{
    public interface IGamePlayer
    {
        int Id { get; }
        string Name { get; }
        void SetOccupied(int cellId);
        void Click(int cellId);
    }
}