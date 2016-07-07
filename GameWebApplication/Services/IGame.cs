using GameWebApplication.Models;

namespace GameWebApplication.Services
{
    public interface IGame
    {
        Scores Scores { get; }
        IGamePlayer Player { get; }

        //IGamePlayer[] Players { get; }

        int CellsCount { get; }

        int PlayersCount { get; }

        void Start();
        void Stop();
    }
}