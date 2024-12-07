using game.Models.interfaces;
using Microsoft.JSInterop;

namespace game.Models.units
{
    public class Bat : AbstractUnit
    {
        public Bat(int initialX, int initialY, IMelee atkManager, MapToGrid map, IRenderer renderer, IGameManager gameManager) : base(initialX, initialY, map, renderer, gameManager)
        {
            frameTime = 0.1f;
            //animationName = "bat";
            lastFrame = 2;
            speed = 2;
            tempSpeed = 2;
            Hp = 100;

            InitTempValues();
        }

        public override void Update()
        {
            base.Update();

        }
    }
}
