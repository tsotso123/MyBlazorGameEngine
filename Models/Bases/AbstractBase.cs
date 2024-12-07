using game.Models.interfaces;
using Microsoft.AspNetCore.Components.RenderTree;

namespace game.Models.Bases
{
    public class AbstractBase : ISortable
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Hp { get; set; }
        public MapToGrid? Map { get; set; }

        public IRenderer? Renderer { get; set; }
        public string ImagePath { get; set; }

        public IGameManager GameManager { get; set; }

        public int FrameCounter { get; set; }

        public float HeightOffset { get; set; }

        public AbstractBase(int initialX, int initialY, MapToGrid map, IRenderer renderer, string imagePath, IGameManager gm)
        {
            Map = map;
            Renderer = renderer;
            X = initialX;
            Y = initialY;
            ImagePath = imagePath;
            GameManager = gm;
            FrameCounter = 0;

        }

        public void Hurt(int damage, int knockBackAmount, float xDir, float yDir, float knockSpeed)
        {
            Hp -= damage;

        }


        public virtual void Update()
        {
            if (Hp <= 0)
            {
                GameManager.Death(this);
            }

        }

        public void Draw(float elapsedTime)
        {

            //await Renderer!.Draw(elapsedTime, X, Y, ImagePath+"/"+ImagePath+"_"+ FrameCounter + ".png",128,128, false);
            Renderer!.DrawBatch(elapsedTime, X, Y, ImagePath + "/" + ImagePath + "_" + FrameCounter + ".png", false, 128, 128);
        }

        public int SortingCompare(ISortable other)
        {
            //float MyYToCompare = Y + 50;
            //if(MyYToCompare>other.Y)
            //{
            //    return 1;
            //}
            //if (MyYToCompare<other.Y)
            //{
            //    return -1;
            //}
            //return 0;

            if (other is AbstractBase)
            {
                // Add 50 to Y for AbstractBase before comparison
                return (Y + HeightOffset).CompareTo(other.Y + HeightOffset);
            }
            else
            {
                // Regular comparison with AbstractUnit
                return (Y + HeightOffset).CompareTo(other.Y);
            }
        }
    }
}
