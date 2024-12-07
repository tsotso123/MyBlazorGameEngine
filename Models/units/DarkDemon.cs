using game.Models.interfaces;
using Microsoft.JSInterop;

namespace game.Models.units
{
    public class DarkDemon : AbstractUnit
    {
        IMelee atkManager;
        public DarkDemon(int initialX, int initialY, IMelee atkManager, MapToGrid map, IRenderer renderer, IGameManager gameManager) : base(initialX, initialY, map, renderer, gameManager)
        {
            frameTime = 0.1f;
            lastFrame = 5;

            speed = 2f;
            //tempSpeed = speed;


            Damage = 33;
            Hp = 100;
            attackKnockBack = 0;

            this.atkManager = atkManager;

            atkLastFrame = 2;
            atkCooldown = 16;
            Range = 14;

            //tempRange = Range;


            PostMovementActions.Enqueue(AfterInitialMovement);

            atkFrameState = new Dictionary<int, bool>() {
                { 1,false}
            };

            InitTempValues();
        }

        private void AfterInitialMovement()
        {
            MoveTo(1400, Y);
        }
        public override void Update()
        {

            if (atkManager!.CheckIfEnemiesInMeleeRange(this))
            {
                if (!atkOnCooldown)
                {
                    TransitionAnimationToAtk();

                    if (atkFrameState!.ContainsKey(frameCounter))
                    {

                        if (frameCounter == 1)
                        {
                            attackKnockBack = 18;
                            attackKnockSpeed = 2f;

                        }
                        else
                        {

                            attackKnockBack = 0;
                            attackKnockSpeed = 0;

                        }


                        if (atkFrameState![frameCounter] == false)
                        {
                            atkManager.AttackMelee(this); // this was outside
                            atkFrameState[frameCounter] = true;
                        }


                    }

                }


            }

            else
            {



                if (atkManager!.PotentialEnemies != null && atkManager.PotentialEnemies.Count > 0)
                {

                    int i;
                    for (i = 0; i < atkManager.PotentialEnemies.Count; i++)
                    {
                        if (atkManager.PotentialEnemies[i].AmIEnemy != AmIEnemy && atkManager.PotentialEnemies[i] != this)
                        {
                            MoveTo(atkManager.PotentialEnemies[i].X, atkManager.PotentialEnemies[i].Y);
                            break;
                        }
                    }

                }

            }


            base.Update();



        }
    }
}
