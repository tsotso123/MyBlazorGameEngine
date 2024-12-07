using game.Models.interfaces;

namespace game.Models.ui
{
    public class UiButton : UiElement
    {
        public UiButton(int x, int y, int width, int height, IRenderer renderer) : base(x, y, width, height, renderer)
        {
            OnClickAction += ButtonClicked;
        }

        public virtual void ButtonClicked()
        {


        }
    }
}
