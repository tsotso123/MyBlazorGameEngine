using game.Models.interfaces;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Drawing;

namespace game.Models.ui
{
    public abstract class UiElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        IRenderer Renderer { get; set; }
        public string? ImagePath { get; set; }

        protected int FrameCounter { get; set; }

        protected float FrameTime { get; set; }
        protected float FrameTracker { get; set; }

        public string? AnimationName { get; set; }

        public int LastFrame { get; set; }

        protected UiElement(int x, int y, int width, int height, IRenderer renderer)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Renderer = renderer;

            InputPropagator.OnClickAction += CheckIfClickedOnElement;
        }
        public bool Clicked { get; set; } = false;
        public virtual void CheckIfClickedOnElement(int x, int y)
        {
            if (x <= X + Width && x >= X && y <= Y + Height && y >= Y)
            {
                Console.Out.WriteLine("clicked on element");
                Clicked = true;
            }
            else
            {
                Console.WriteLine("clicked at:" + x + "-" + y);
                Console.WriteLine("but element is at:" + X + "-" + Y);
            }

        }

        public delegate void OnClickActionDelegate(); // Delegate to allow async tasks
        public OnClickActionDelegate? OnClickAction { get; set; }


        public virtual void Draw(float elapsedTime)
        {
            //await Renderer.DrawToUiLayer(elapsedTime, X, Y, ImagePath!, Width, Height, false);
            Renderer.DrawBatch(elapsedTime, X, Y, "Ui/"+ImagePath!, false, Width, Height, 0);
        }

        public virtual void Update()
        {
            if (Clicked)
            {
                if (OnClickAction != null)
                {
                    OnClickAction.Invoke();
                }
                Clicked = false;
            }
        }
    }
}
