using game.Models.interfaces;
using Microsoft.JSInterop;

namespace game.Models.units
{
    public class Assassin : AbstractUnit
    {
        IMelee? atkManager;

        public Assassin(int initialX, int initialY, IMelee atkManager, MapToGrid map, IRenderer renderer, IGameManager gameManager) : base(initialX, initialY, map, renderer, gameManager)
        {
            Cost = 0;

            frameTime = 0.1f;
            //animationName = "assassin";

            lastFrame = 5;

            atkLastFrame = 8;

            speed = 2f;
            //tempSpeed = speed;

            Aoe = 2;

            Damage = 33;
            Hp = 100;

            attackKnockBack = 0;

            MoveWithArrowKeys = true;

            this.atkManager = atkManager;

            atkCooldown = 16;

            Range = 10;

            //tempRange = Range;


            PostMovementActions.Enqueue(AfterInitialMovement);

            atkFrameState = new Dictionary<int, bool>() {
                { 3,false},
                { 4,false},
                { 7,false}
            };

            InitTempValues();
        }

        private void AfterInitialMovement()
        {
            MoveTo(1400, Y);// go to enemy base, was uncommented
        }

        // any attack/damage logic needs to be at overriden Update method, as that is based on current game state
        // basically, any logic that depends on other units, needs to synched (at Update method)
        public override void Update()
        {

            if (atkManager!.CheckIfEnemiesInMeleeRange(this))
            {
                if (!atkOnCooldown)
                {
                    TransitionAnimationToAtk();

                    //if (atkFrameState!.ContainsKey(frameCounter))
                    if (frameCounter == 4 || frameCounter == 3 || frameCounter == 7)
                    {

                        if (frameCounter == 7)
                        {
                            attackKnockBack = 2;
                            attackKnockSpeed = 2f;

                        }
                        else
                        {
                            attackKnockSpeed = 0.5f;
                            Range += 0.6f; // works if range is twice the attackKnockSpeed

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

            //else if (attacking)
            //{
            //    frameCounter = 0;
            //    attacking = false;
            //    frameTracker = 0;
            //}

        }
    }
}
