using game.Models.interfaces;
using game.Models.units;
using Microsoft.AspNetCore.Components.RenderTree;
namespace game.Models
{
    public class Projectile
    {
        public float X { get; set; }      
        public float Y { get; set; }
        public float MoveToX { get; set; }
        public float MoveToY { get; set; }
        public float Speed { get; set; } = 5;
        public bool AmIEnemy { get; set; }
        public IProjectile AtkManager { get; set; }
        public int Size { get; set; } = 16;
        public int Damage { get; set; } = 10;
        public int AttackKnockBack { get; set; } = 2;
        public float AttackKnockSpeed { get; set; } = 2;
        public int Aoe { get; set; } = 1;
        public float TimeToLive { get; set; } = 4;
        public int AtkCooldown { get; set; } = 16;
        public int AtkCooldownTracker { get; set; } = 0;
        public bool AtkOnCooldown { get; set; }
        public IGameManager GameManager { get; set; }
        public float Timer { get; set; } = 0;
        public string ImgPath { get; set; }
        public IRenderer Renderer { get; set; }
        public bool FlipAnimation { get; set; }
        public float Rotation { get; set; }
        public int LastFrame { get; set; } = 1;

        public bool ProjHit { get; set; } = false;
        public float InitialX { get; set; }
        public float InitialY { get; set; }
        public List<AbstractUnit> AlreadyHitByProjectilesUnits { get; set; }
        public List<int> UnitsHitCooldownTracker { get; set; }
        public Projectile(float x,float y, float dirX,float dirY, string ImgPath,IProjectile atkManager,IGameManager GameManager,IRenderer Renderer)
        {
            X = x;
            Y = y;
            MoveToX = dirX;
            MoveToY = dirY;
            AtkManager = atkManager;
            this.GameManager = GameManager;
            this.ImgPath = ImgPath;
            this.Renderer = Renderer;
            InitialX = x; InitialY = y;

            if (dirX>x)
            {
                FlipAnimation = false;
            }
            else
            {
                FlipAnimation = true;
            }

            if (Math.Abs(MoveToX) < Speed)
            {
                Speed = Math.Abs(MoveToX);
            }

            CalculateRotation();
            CalculateLine();

            //if (InitialX > MoveToX)
            //{
            //    //MoveToX -= 1000;

            //    MoveToY = M * MoveToX + B;
            //    //MoveToY *= -1;
            //}
            //else if (InitialX < MoveToX)
            //{
            //    //MoveToX += 1000;

            //    MoveToY = M * MoveToX + B;

            //}
            

            AlreadyHitByProjectilesUnits = [];
            UnitsHitCooldownTracker = [];


        }
        float M;
        float B;
        private void CalculateLine() {
            this.M = (MoveToY - Y) / (MoveToX - X);
            this.B = MoveToY - M*MoveToX;
        }
        private void CalculateRotation()
        {
            float A = Math.Abs(Math.Abs(Y)- Math.Abs(MoveToY));
            //float A = Y-MoveToY;
            float C = (float)AtkManager.DistanceBetween2Points(MoveToX,MoveToY,X,Y);
            if (MoveToX>X)
            {
                C = C * -1;
            }

            if (MoveToY>Y)
            {
                C = C * -1;
            }

            
            float SinAlpha = A / C;
            
            Rotation = (float)Math.Asin(SinAlpha);
        }
        public void UpdateToEnqueue()
        {
            // also death of projectile if hit enemies are exceeding/equal aoe
            if (Timer>=TimeToLive)
            {
                GameManager.Death(this);  
            }          

            for (int i = 0;i< UnitsHitCooldownTracker.Count;i++)
            {
                UnitsHitCooldownTracker[i]++;
                if (UnitsHitCooldownTracker[i]==AtkCooldown)
                {
                    AlreadyHitByProjectilesUnits.RemoveAt(i);
                    UnitsHitCooldownTracker.RemoveAt(i);
                }
            }

            //if (AtkOnCooldown)
            //{
            //    AtkCooldownTracker++;
            //    if (AtkCooldownTracker == AtkCooldown)
            //    {
            //        AtkOnCooldown = false;
            //        AtkCooldownTracker = 0;
            //        AlreadyHitByProjectilesUnits.Clear();
            //    }
            //}

            Move();
            
        }

        protected void Move()
        {
            
            bool reachedX = false;
            bool reachedY = false;

            if (InitialX < MoveToX)
            {
                X += Speed;
                if (X >= MoveToX)
                {
                    reachedX = true;
                    
                }
            }
            if (InitialX > MoveToX)
            {
                X -= Speed;
                if (X <= MoveToX)
                {
                    reachedX=true;

                }
            }
            //if (X == MoveToX)
            //{
            //    if (X > InitialX)
            //    {
            //        X += Speed;
            //    }
            //}
            //else
            //{
            //    if (X < MoveToX)
            //    {
            //        X += Speed;
            //        if (X >= MoveToX)
            //        {
            //            reachedX = true;
            //        }
            //    }
            //    if (X > MoveToX)
            //    {
            //        X -= Speed;
            //        if (X <= MoveToX)
            //        {
            //            reachedX = true;
            //        }
            //    }
            //}            
            

            if (Math.Abs(Math.Abs(M * X + B)-Math.Abs(Y)) > Speed || (float.IsInfinity(M) || float.IsInfinity(B)))
            {
                if (InitialY < MoveToY)
                {
                    Y += Speed;
                    if (Y >= MoveToY)
                    {
                        reachedY = true;
                    }
                }
                if (InitialY > MoveToY)
                {
                    Y -= Speed;
                    if (Y <= MoveToY)
                    {
                        reachedY = true;
                    }
                }

                if ((!float.IsInfinity(M) && !float.IsInfinity(B)))
                {
                    X = (Y - B) / M;
                }                  
            }
            else
            {
                Y = M * X + B;
                if (InitialY < MoveToY)
                {                   
                    if (Y >= MoveToY)
                    {
                        reachedY = true;
                    }
                }
                if (InitialY > MoveToY)
                {                
                    if (Y <= MoveToY)
                    {
                        reachedY = true;
                    }
                }
            }
            
            

            //if (Y < MoveToY)
            //{
            //    //Y += Speed;
            //    Y += ySmoother;
            //    if (Y >= MoveToY)
            //    {
            //        reachedY = true;
            //    }
            //}
            //if (Y > MoveToY)
            //{
            //    //Y -= Speed;
            //    Y -= ySmoother;
            //    if (Y <= MoveToY)
            //    {
            //        reachedY = true;
            //    }
            //}

            if (X == MoveToX)
            {
                reachedX = true;
            }
            if (Y == MoveToY)
            {
                reachedY = true;
            }

            if (reachedX && reachedY)
            {
                
                Speed = 0;
                Timer = Timer + TimeToLive; // this ensures the death happens next frame

                //float tempM;

                //if (InitialX > MoveToX)
                //{
                //    MoveToX -= 1000;
                //    tempM = M;// * -1;
                //    MoveToY = tempM * MoveToX + B;
                //    //MoveToY *= -1;
                //}
                //else if (InitialX < MoveToX)
                //{
                //    MoveToX += 1000;
                //    tempM = M;
                //    MoveToY = tempM * MoveToX + B;

                //}


                //MoveToY = tempM * MoveToX + B;


                // calc Y for M 

                //MoveToX *= 2;
                //MoveToY *= 2;

            }
        }

        public void Update(float elapsedTime=0.016f)
        {
            Timer += elapsedTime;

            //if (!AtkOnCooldown)
            //{
            ProjHit = AtkManager.ProjectileHit(this);

                //AtkOnCooldown = true;
            //}
            
                

            GameManager.PostFrameActions.Enqueue(UpdateToEnqueue);
        }

        public void Draw(float elapsedTime)
        {
            Renderer.DrawBatch(elapsedTime, X, Y, ImgPath+"/"+ImgPath+"_0.png", FlipAnimation,Size,Size,Rotation);
        }
    }
}
