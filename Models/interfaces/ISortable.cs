namespace game.Models.interfaces
{
    public interface ISortable
    {
        public float Y { get; }
        public float X { get; }

        public float HeightOffset { get; }

        public int SortingCompare(ISortable other);
    }
}
