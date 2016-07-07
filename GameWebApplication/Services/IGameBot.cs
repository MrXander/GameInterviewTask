namespace GameWebApplication.Services
{
    public interface IGameBot : IGamePlayer
    {               
        void Start();
        void Stop();
    }
}