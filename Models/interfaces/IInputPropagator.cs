namespace game.Models.interfaces
{
    public interface IInputPropagator
    {
        public Dictionary<string, bool> _keyStates { get; set; }

        public static Action<int, int>? OnClickAction { get; set; }

        public Task HandleClick(int x, int y);

        public Task HandleKeyDown(string key);

        public Task HandleKeyUp(string key);

        public void SetKeyState(string key, bool isPressed);
    }
}
