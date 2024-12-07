using game.Models.interfaces;
using game.Models.Bases;
using Microsoft.JSInterop;

namespace game.Models.units
{
    public abstract class AbstractUnit : ISortable
    {
        public float X { get; set; }
        public float Y { get; private set; }

        public float PreviousX { get; set; }
        public float PreviousY { get; set; }

        protected float speed;


        public float Range { get; set; }
        public int Hp { get; set; }
        public int Damage { get; set; }

        public int Aoe { get; set; } = 1;

        protected int frameCounter = 0;
        protected float frameTime = 0.8f;
        protected float frameTracker = 0;

        // dont touch this
        public string? animationName;
        // dont touch this
        public int lastFrame;


        public bool attacking;
        public int atkLastFrame;

        protected Dictionary<int, bool>? atkFrameState; // because of a 60 fps, we might be on a certain frame, for much longer

        protected bool moving;

        MapToGrid? map;

        bool flipAnimation = false;

        protected IRenderer? renderer;

        float MoveToX = 0;
        float MoveToY = 0;

        protected bool MoveWithArrowKeys = false;

        public bool knockedBack = false;
        int knockBackAmount = 0;
        float knockSpeed = 0;
        float knockDirX = 0;
        float knockDirY = 0;

        public int attackKnockBack = 0;
        public float attackKnockSpeed = 0;

        protected float tempSpeed;
        protected float tempRange;

        public int atkCooldown;
        public int atkCooldownTracker = 0;
        public bool atkOnCooldown = false;

        public float HeightOffset { get; set; }

        public IGameManager Gamemanager { get; set; }

        public Queue<Action> PostMovementActions { get; set; }
        protected bool initialMovementWhenSpawned;

        public int Cost { get; set; }

        public bool AmIEnemy { get; set; }

        public int UpperAtkRange = 32;
        public AbstractUnit(int initialX, int initialY, MapToGrid map, IRenderer renderer, IGameManager gameManager)
        {
            X = initialX;
            Y = initialY;

            this.renderer = renderer;

            PreviousX = X;
            PreviousY = Y;

            //this.map = map;

            //map.InsertAt(X, Y, this);

            tempSpeed = speed;

            Gamemanager = gameManager;

            PostMovementActions = new Queue<Action>();

            MoveTo(X, Y + 64);
            initialMovementWhenSpawned = true;
        }
        public void InsertAtMap(MapToGrid map)
        {
            this.map = map;
            map.InsertAt(X, Y, this);
        }
        protected void InitTempValues()
        {
            tempRange = Range;
            tempSpeed = speed;
        }
        public void MoveTo(float x, float y)
        {
            MoveToX = x;
            MoveToY = y;
        }
        public void Hurt(int damage, int knockBackAmount, float xDir, float yDir, float knockSpeed)
        {
            Hp -= damage;

            if (!AmIEnemy)
            {
                //Console.WriteLine("hp:" + Hp);
            }

            if (!knockedBack)
            {
                knockedBack = true;
                knockDirX = xDir;
                knockDirY = yDir;
                this.knockBackAmount = knockBackAmount;
                this.knockSpeed = knockSpeed;

                //if (knockDirX>X)
                //{
                //    flipAnimation = false;
                //}
            }

            // if already knocked back, stronger knock back prevails

            //else
            //{
            //    if (knockBackAmount>this.knockBackAmount && knockSpeed>=this.knockSpeed)
            //    {
            //        knockDirX = xDir;
            //        knockDirY = yDir;
            //        this.knockBackAmount = knockBackAmount;
            //        this.knockSpeed = knockSpeed;
            //    }

            //}

        }

        public void Knockback()
        {
            if (knockDirY > Y)
            {
                Y -= knockSpeed;//speed * 0.4f;
            }
            if (knockDirY < Y)
            {
                Y += knockSpeed;//speed * 0.4f;
            }
            if (knockDirX > X)
            {
                X -= knockSpeed;//speed * 0.4f;
            }
            if (knockDirX <= X)
            {
                X += knockSpeed;//speed * 0.4f;
            }


            knockBackAmount--;
            if (knockBackAmount <= 0)
            {
                knockedBack = false;
            }
        }

        public virtual void UpdateToEnqueue()
        {
            if (Hp <= 0)
            {
                if (AmIEnemy && Hp <= 0)
                {
                    Console.WriteLine("ENEMY DIED");
                }
                else if (!AmIEnemy && Hp <= 0)
                {
                    Console.WriteLine("ALLY DIED");
                }

                map!.RemoveAt(X, Y, this);
                map!.RemoveAt(PreviousX, PreviousY, this); // making sure
                Gamemanager.Death(this);
                return;
            }

            moving = false;

            if (knockedBack)
            {
                Knockback();
            }

            if (!knockedBack && !attacking && !atkOnCooldown)
            {
                if (MoveToX != 0 || MoveToY != 0)
                {
                    moving = true;
                    Move();
                }


                if (MoveWithArrowKeys)
                {
                    ArrowKeysMovement();
                }
            }


            if (PreviousX < X)
            {
                flipAnimation = false;
            }
            else if (PreviousX > X)
            {
                flipAnimation = true;
            }

            if (atkOnCooldown)
            {
                atkCooldownTracker++;
                if (atkCooldownTracker == atkCooldown)
                {

                    atkOnCooldown = false;
                    atkCooldownTracker = 0;
                }

            }

            UpdatePositionOnGridMap();

        }
        public virtual void Update()
        {
            Gamemanager.PostFrameActions.Enqueue(UpdateToEnqueue);

            //if (Hp <= 0)
            //{
            //    //map!.RemoveAt(X, Y, this);
            //    //map!.RemoveAt(PreviousX, PreviousY, this); // making sure

            //    Gamemanager.PostFrameActions.Enqueue(() =>
            //    {
            //        map!.RemoveAt(X, Y, this);
            //        map!.RemoveAt(PreviousX, PreviousY, this); // making sure
            //        Gamemanager.Death(this);

            //    });


            //    return;
            //}


            //moving = false;

            //if (knockedBack)
            //{
            //    Gamemanager.PostFrameActions.Enqueue(() =>
            //    {
            //        Knockback();
            //    });
            //    //Knockback();
            //}

            //if (!knockedBack && !attacking && !atkOnCooldown)
            //{
            //    if (MoveToX != 0 || MoveToY != 0)
            //    {
            //        moving = true;
            //        Gamemanager.PostFrameActions.Enqueue(() =>
            //        {
            //            Move();
            //        });
            //        //Move();
            //    }

            //    //if (MoveWithArrowKeys)
            //    //{
            //    //    ArrowKeysMovement();
            //    //}
            //}

            //if (PreviousX < X)
            //{
            //    flipAnimation = false;
            //}
            //else if (PreviousX > X)
            //{
            //    flipAnimation = true;
            //}

            //if (atkOnCooldown)
            //{
            //    atkCooldownTracker++;
            //    if (atkCooldownTracker == atkCooldown)
            //    {
            //        atkOnCooldown = false;
            //        atkCooldownTracker = 0;
            //    }

            //}

            ////UpdatePositionOnGridMap();
            //Gamemanager.PostFrameActions.Enqueue(() =>
            //{
            //    UpdatePositionOnGridMap();
            //});

        }

        protected void Move()
        {

            bool reachedX = false;
            bool reachedY = false;
            if (X < MoveToX)
            {
                X += speed;
                if (X >= MoveToX)
                {
                    reachedX = true;
                }
            }
            if (X > MoveToX)
            {
                X -= speed;
                if (X <= MoveToX)
                {
                    reachedX = true;
                }
            }
            if (Y < MoveToY)
            {
                Y += speed;
                if (Y >= MoveToY)
                {
                    reachedY = true;
                }
            }
            if (Y > MoveToY)
            {
                Y -= speed;
                if (Y <= MoveToY)
                {
                    reachedY = true;
                }
            }

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
                moving = false;
                MoveToX = 0;
                MoveToY = 0;
                if (initialMovementWhenSpawned)
                {
                    initialMovementWhenSpawned = false;
                    while (PostMovementActions.Count > 0)
                    {
                        PostMovementActions.Dequeue()?.Invoke();
                    }
                }
            }
        }
        protected void ArrowKeysMovement()
        {
            if (InputPropagator._keyStates["ArrowUp"])
            {
                Y -= speed;
                moving = true;
            }
            if (InputPropagator._keyStates["ArrowDown"])
            {
                Y += speed;
                moving = true;
            }
            if (InputPropagator._keyStates["ArrowLeft"])
            {
                X -= speed;
                moving = true;
            }
            if (InputPropagator._keyStates["ArrowRight"])
            {
                X += speed;
                moving = true;
            }
        }
        protected void UpdatePositionOnGridMap()
        {

            int cell_ind_x = (int)(X / map!.cellSizeWidth) * map.cellSizeWidth;
            int cell_ind_y = (int)(Y / map.cellSizeHeight) * map.cellSizeHeight;
            int cell_ind_prev_x = (int)(PreviousX / map.cellSizeWidth) * map.cellSizeWidth;
            int cell_ind_prev_y = (int)(PreviousY / map.cellSizeHeight) * map.cellSizeHeight;

            if (cell_ind_prev_x != cell_ind_x || cell_ind_prev_y != cell_ind_y)
            {

                map.RemoveAt(PreviousX, PreviousY, this);

                PreviousX = X;
                PreviousY = Y;

                map.InsertAt(X, Y, this);
            }
        }

        public void Draw(float elapsedTime)
        {

            string animationToDraw = animationName!;
            int animationLastFrame;

            string atk = "";
            if (attacking)
            {
                atk = "atk";
                animationLastFrame = atkLastFrame;
            }
            else
            {
                animationLastFrame = lastFrame;

                if (!moving)
                {
                    frameCounter = 0;
                }
            }


            
            renderer!.DrawBatch(elapsedTime, X, Y, animationToDraw + "/" + animationToDraw + atk + "_" + frameCounter + ".png", flipAnimation);

            if (frameTracker > frameTime)
            {
                frameCounter++;
                if (frameCounter == animationLastFrame)
                {
                    frameCounter = 0;
                    if (attacking)
                    {
                        attacking = false;
                        speed = tempSpeed;
                        atkOnCooldown = true;
                        Range = tempRange;
                        resetAtkFrameState();
                       
                    }
                }

                frameTracker = 0;
            }

            frameTracker += elapsedTime;
        }
        private void resetAtkFrameState()
        {
            foreach (var key in atkFrameState!.Keys.ToList()) // Use ToList to avoid modifying the collection during iteration
            {
                atkFrameState[key] = false;
            }
        }
        protected void stopAttacking()
        {
            speed = tempSpeed;
            attacking = false;
            frameCounter = 0;
            frameTracker = 0;
        }
        protected void TransitionAnimationToAtk()
        {
            if (!attacking)
            {
                frameTracker = 0;
                frameCounter = 0;
                attacking = true;
                speed = 0;
            }
            

        }

        public int SortingCompare(ISortable other)
        {
            if (other is AbstractBase)
                return Y.CompareTo(other.Y + other.HeightOffset);
            else
                return Y.CompareTo(other.Y);


        }


    }
}
