using game.Models.interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace game.Models
{
    public class JsRenderer : IRenderer
    {
        List<ToDraw> _toDraw;

        public IJSRuntime runtime;

        public JsRenderer(IJSRuntime runtime) {
            this.runtime = runtime;
            _toDraw = new List<ToDraw>();
        }

        public async Task FinalDraw()
        {
            await runtime.InvokeVoidAsync("finalDraw");
        }

        public async Task DrawToUiLayer(float elapsedTime, float x, float y, string imgToRender, int width,int height,bool flipAnimation)
        {
            await runtime.InvokeVoidAsync("drawOnUi", x, y, imgToRender,width,height, flipAnimation);
        }
        public async Task Draw(float elapsedTime, float x, float y, string imgToRender, int width, int height,bool flipAnimation)
        {
            await runtime.InvokeVoidAsync("drawWithWidthAndHeight", x, y, imgToRender, width, height, flipAnimation);
            //await runtime.InvokeVoidAsync("webGLRenderImage", x, y, imgToRender, flipAnimation);
        }

        public async Task DrawAll()
        {
            if (_toDraw.Count>0)
            {

                string json = JsonSerializer.Serialize(_toDraw);
                await MyJsInterop.drawAll(json);

                _toDraw.Clear();
                
            }
            else
            {
                return;
            }    
        }
        public void DrawBatch(float elapsedTime, float x, float y, string imgToRender, bool flipAnimation,int width=0,int height=0,float rotate=0)
        {
            _toDraw.Add(new ToDraw { elapsedTime=elapsedTime,X=x,Y=y,animationToDraw=imgToRender,flipAnimation=flipAnimation,width=width,height=height,rotate=rotate });
        }
        public async Task Draw(float elapsedTime,float x,float y,string imgToRender,bool flipAnimation)
        {
            await runtime.InvokeVoidAsync("draw", x, y, imgToRender, flipAnimation);

            //await runtime.InvokeVoidAsync("webGLRenderImage", x, y, imgToRender, flipAnimation);


        }

        public async Task PreLoadImage(string imagePath)
        {
            await runtime.InvokeVoidAsync("preloadImage", imagePath);
        }

        public async Task Clear(List<ISortable> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                await runtime.InvokeVoidAsync("clearAt", units[i].X, units[i].Y,40,40);
            }
        }

        public async Task Clear()
        {
            await runtime.InvokeVoidAsync("clearCanvas");
            //await runtime.InvokeVoidAsync("webGlClear");
        }
    }
}
