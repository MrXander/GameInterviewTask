using GameWebApplication.Models;
using Microsoft.AspNet.SignalR;

namespace GameWebApplication.Services
{
    public class ClientGameHub : Hub, IClientGameHub
    {
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
            Clients.Caller.botGotPoint(cellId);
        }

        public void GameOverClient(Scores scores)
        {            
            Clients.Caller.gameOver(scores);            
        }        
    }

    public interface IClientGameHub
    {        
        void ResetOccupiedCellClient(int cellId);        
        void SetOccupiedCellClient(int cellId);        
        void ClickClient(int cellId);
        void GameOverClient(Scores scores);
    }
}