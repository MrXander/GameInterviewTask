namespace GameWebApplication.Models
{
    public class Cell
    {
        public int Id { get; set; }
        public bool IsOccupied => OccupiedBy != null;
        public IGamePlayer OccupiedBy { get; set; }
    }
}