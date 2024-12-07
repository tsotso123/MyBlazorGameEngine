using Microsoft.JSInterop;

namespace game.Models.interfaces
{
    public interface IRenderer
    {
        public Task DrawAll();
        public void DrawBatch(float elapsedTime, float x, float y, string imgToRender, bool flipAnimation,int width=0,int height=0,float rotate=0);
        public Task Draw(float elapsedTime, float x, float y, string imgToRender, bool flipAnimation);
        public Task Draw(float elapsedTime, float x, float y, string imgToRender, int width, int height, bool flipAnimation);

        public Task PreLoadImage(string image);

        public Task DrawToUiLayer(float elapsedTime, float x, float y, string imgToRender,int width,int height, bool flipAnimation);
        public Task Clear();
        public Task Clear(List<ISortable> units);
        public Task FinalDraw();
    }
}
