using Microsoft.JSInterop;

namespace game.Models.interfaces
{
    public interface IUnit
    {
        public int InitialX { get; set; }
        public int InitialY { get; set; }

        public int Speed { get; set; }

        public int PreviousX { get; set; }
        public int PreviousY { get; set; }

        public int Range { get; set; }
        public int Hp { get; set; }
        public int Damage { get; set; }

        public int Aoe { get; set; }

        public int FrameCounter { get; set; }
        protected float FrameTime { get; set; }
        float FrameTracker { get; set; }

        // dont touch this
        public string? AnimationName { get; set; }
        // dont touch this
        public int LastFrame { get; set; }

        public int AtkLastFrame { get; set; }

        public bool Attacking { get; set; }
        
        protected bool Moving { get; set; }
    }
}
