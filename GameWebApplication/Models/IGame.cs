namespace GameWebApplication.Models
{
    public interface IGame
    {
        Scores Scores { get; }
        IGamePlayer Player { get; }

        int CellsCount { get; }

        int PlayersCount { get; }

        void Start();
        void Stop();
    }
}