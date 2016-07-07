namespace GameWebApplication.Models
{
    public interface IGameBot : IGamePlayer
    {
        void Start();
        void Stop();
    }
}