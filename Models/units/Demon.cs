using game.Models.interfaces;
using Microsoft.JSInterop;
using System.Xml;

namespace game.Models.units
{
    public class Demon : AbstractUnit
    {
        IRanged atkManager;
        public Demon(int initialX, int initialY, IRanged atkManager, MapToGrid map, IRenderer renderer, IGameManager gameManager) : base(initialX, initialY, map, renderer, gameManager)
        {
            //animationName = "demon";
            frameTime = 0.1f;
            lastFrame = 5;

            atkLastFrame = 2;

            Hp = 100;

            
            Aoe = 6;

            speed = 2f;
            //tempSpeed = speed;

            Damage = 10;
            atkCooldown = 60;

            Range = 15;
            //tempRange = Range;

            attackKnockBack = 9;
            attackKnockSpeed = 2f;

            this.atkManager = atkManager;

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
            if (atkManager.ScanRange(this))
            {
                //if (!AmIEnemy)
                //{
                //    Console.WriteLine("");
                //}

                if (!atkOnCooldown)
                {
                    TransitionAnimationToAtk();
                    if (atkFrameState!.ContainsKey(frameCounter))
                    {
                        if (atkFrameState![frameCounter] == false)
                        {
                            //atkManager.shoot(this);
                            Projectile projectile = new Projectile(X, Y, atkManager.EnemyToShootAt.X, atkManager.EnemyToShootAt.Y, "projectiles",(IProjectile)atkManager, Gamemanager, renderer!);
                            projectile.AmIEnemy = AmIEnemy;
                            projectile.AttackKnockBack = attackKnockBack;
                            projectile.AttackKnockSpeed = attackKnockSpeed;
                            projectile.Aoe = 1;
                            //Gamemanager.AddProjectile(projectile);
                            Gamemanager.PostFrameActions.Enqueue(() =>
                            {
                                Gamemanager.AddProjectile(projectile);
                            });

                            atkFrameState[frameCounter] = true;
                        }
                    }

                }
                
            }


            base.Update();

        }
    }
}
