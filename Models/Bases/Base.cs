using game.Models.interfaces;

namespace game.Models.Bases
{
    public class Base : AbstractBase
    {
        public Base(int initialX, int initialY, MapToGrid map, IRenderer renderer, string imagePath, IGameManager gm) : base(initialX, initialY, map, renderer, imagePath, gm)
        {
            Hp = 100;
            HeightOffset = 80;
        }

        //public int CompareTo(AbstractUnit? other)
        //{
        //    if (other!.Y > Y+50)
        //        return -1;
        //    if (other.Y < Y+50)
        //        return 1;
        //    return 0;
        //}

        public override void Update()
        {
            base.Update();

        }
    }

}
