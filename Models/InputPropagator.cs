namespace game.Models
{
    public static class InputPropagator
    {
        public static Dictionary<string, bool> _keyStates= new Dictionary<string, bool>()
            {
                { "ArrowUp", false },
                { "ArrowDown", false },
                { "ArrowLeft", false },
                { "ArrowRight", false }
            };

        public static Action<int, int>? OnClickAction;

        public static Task HandleClick(int x, int y)
        {
            Console.WriteLine("clicked");
            if (OnClickAction != null)
            {
                OnClickAction(x, y); // Invoke the delegate
            }

            return Task.CompletedTask;
        }
        public static Task HandleKeyDown(string key)
        {
            SetKeyState(key, true);
            return Task.CompletedTask;
        }

        public static Task HandleKeyUp(string key)
        {
            SetKeyState(key, false);
            return Task.CompletedTask;
        }

        public static void SetKeyState(string key, bool isPressed)
        {
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = isPressed;
            }
        }
    }
}
