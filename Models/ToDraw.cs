namespace game.Models
{
    public class ToDraw
    {
        public float elapsedTime { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string? animationToDraw { get; set; }
        public bool flipAnimation { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public float rotate { get; set; }
    }
}
