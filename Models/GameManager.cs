using game.Models.interfaces;
using game.Models.ui;
using game.Models.units;
using game.Models.Bases;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks.Dataflow;

namespace game.Models
{
    public class GameManager : IGameManager
    {

        private Dictionary<string, Func<int, int, AttackTypeManager, MapToGrid, IRenderer, IGameManager, AbstractUnit>>? unitFactory;
        public List<AbstractUnit> unitList { get; private set; }
        public List<AbstractUnit> enemyList { get; private set; }
        public List<Projectile> projectilesList { get; private set; }
        public List<AbstractBase> BaseList { get; private set; }
        public List<UiElement> UiElementsList { get; private set; }

        public IRenderer renderer;
        Stopwatch stopwatch;

        //MapToGrid map;
        MapToGrid mapAllies;
        MapToGrid mapEnemies;

        AttackTypeManager attackTypeManager;

        List<string> namesOfUnitsAlreadyPreLoaded;
        public int BaseLocationX { get; set; }
        public int BaseLocationY { get; set; }
        public int EnemyBaseLocationX { get; set; }
        public int EnemyBaseLocationY { get; set; }     

        List<ISortable> GameObjectsToYSort = new List<ISortable>();

        public Queue<Action> PostFrameActions { get; set; }

        public List<Action> ConstantPostFrameActions { get; set; }

        private EnemySpawner enemySpawner;
        private ManaManager manaManager;

        public GameManager(IRenderer renderer)
        {
            this.renderer = renderer;
            stopwatch = new Stopwatch();

            unitList = new List<AbstractUnit>();
            enemyList = new List<AbstractUnit>();

            projectilesList = new List<Projectile>();

            UiElementsList = new List<UiElement>();
            BaseList = new List<AbstractBase>();

            namesOfUnitsAlreadyPreLoaded = new List<string>();

            //map = new MapToGrid(1600,600,32,32);
            mapAllies = new MapToGrid(1600,600,32,32);
            mapEnemies = new MapToGrid(1600,600,32,32);

            attackTypeManager = new AttackTypeManager(mapAllies, mapEnemies);

            GameObjectsToYSort = new List<ISortable>();
            PostFrameActions = new Queue<Action>();
            ConstantPostFrameActions = new List<Action>();

            
            enemySpawner = new EnemySpawner(this, [

                new EnemyWave(0.2f,9,"Demon"),
                // after 5 seconds spawn 3 demon
                 // after 10 seconds spawn 6 assassin... etc

            ]);


            //
            //for (int i = 0; i < 90; i++)
            //{
            //    Assassin s = new Assassin(BaseLocationX + 190 - (i + 1) * 2, BaseLocationY + 64, attackTypeManager, mapAllies, renderer, this);
            //    s.animationName = "Assassin";
            //    AddUnit(s);
            //}
            //

            manaManager = new ManaManager(this);

            BaseLocationX = 50;
            BaseLocationY = 216;

            EnemyBaseLocationX = 1422;
            EnemyBaseLocationY = 216;

            InitializeUnitFactory();

            InitBases();

            SummonUnitButton summonUnitButton = new SummonUnitButton(350,500,64,64,renderer,this,"Assassin",manaManager);
            summonUnitButton.ImagePath = "unit icon bg.png";
            AddUiElement(summonUnitButton);




            
        }

        private void InitBases()
        {
            AddBase(new Base(BaseLocationX, BaseLocationY, mapAllies, renderer, "base", this));

            AddBase(new Base(EnemyBaseLocationX, EnemyBaseLocationY, mapEnemies, renderer, "base", this));
        }

        public async void AddBase(AbstractBase abstractBase)
        {
            await PreLoadAssets(abstractBase.ImagePath!, 1);
            BaseList.Add(abstractBase);
            GameObjectsToYSort.Add(abstractBase);
        }
        public async void AddUiElement(UiElement element)
        {
            await PreLoadUiAssets(element.ImagePath!);
            UiElementsList.Add(element);
        }
        public async void AddUnit(AbstractUnit unit)
        {
            if (!namesOfUnitsAlreadyPreLoaded.Contains(unit.animationName!))
            {
                await PreLoadAssets(unit.animationName!, unit.lastFrame);
                await PreLoadAssets(unit.animationName!, unit.atkLastFrame, "atk");
                namesOfUnitsAlreadyPreLoaded.Add(unit.animationName!);
            }
            unit.AmIEnemy = false;
            unit.InsertAtMap(mapAllies);
            unitList.Add(unit);
            GameObjectsToYSort.Add(unit);
        }

        public async void AddEnemyUnit(AbstractUnit unit)
        {
            if (!namesOfUnitsAlreadyPreLoaded.Contains(unit.animationName!))
            {
                await PreLoadAssets(unit.animationName!, unit.lastFrame);
                await PreLoadAssets(unit.animationName!, unit.atkLastFrame, "atk");
                namesOfUnitsAlreadyPreLoaded.Add(unit.animationName!);
            }
            unit.AmIEnemy = true;
            unit.InsertAtMap(mapEnemies);
            enemyList.Add(unit);
            GameObjectsToYSort.Add(unit);
            
        }

        public async void AddProjectile(Projectile projectile)
        {
            if (!namesOfUnitsAlreadyPreLoaded.Contains(projectile.ImgPath))
            {
                await PreLoadAssets(projectile.ImgPath, projectile.LastFrame);
                //await PreLoadAssets(projectile.ImgPath, projectile.atkLastFrame, "atk");

                //namesOfUnitsAlreadyPreLoaded.Add(projectile.ImgPath);
            }
            projectilesList.Add(projectile);
        }

        private async Task PreLoadUiAssets(string image)
        {
            //await SkiaRenderer.jsRenderer.PreLoadImage("assets/Ui/" + image);//

            await renderer.PreLoadImage("assets/Ui/" + image);
        }
        private async Task PreLoadAssets(string animationName, int lastFrame,string atk="")
        {
            for (int i = 0; i < lastFrame; i++)
            {
                //await jsRuntime.InvokeVoidAsync("preloadImage", "assets/" + animationName + "/" + animationName + atk + "_" + i + ".png");

                //await SkiaRenderer.jsRenderer.PreLoadImage("assets/" + animationName + "/" + animationName + atk + "_" + i + ".png");//

                await renderer.PreLoadImage("assets/" + animationName + "/" + animationName + atk + "_" + i + ".png");
            }
        }
        
        private void InitializeUnitFactory()
        {
            unitFactory = new Dictionary<string, Func<int,int,AttackTypeManager,MapToGrid,IRenderer,IGameManager,AbstractUnit>>();

            // Discover all types that inherit from AbstractUnit
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(AbstractUnit)))
                {                   
                    // Add a lambda that creates an instance of this type, ctor parameters (arg,arg2 ...) are just for signature for ctor
                    unitFactory[type.Name] = (arg,arg2,arg3,arg4,arg5,arg6) => (AbstractUnit)Activator.CreateInstance(type,arg,arg2,arg3,arg4,arg5,arg6)!;
                }
            }
        }

        // Now, whenever you need to instantiate a unit:
        public AbstractUnit CreateUnit(string unitType,int initX,int initY)
        {
            if (unitFactory!.TryGetValue(unitType, out var factoryMethod))
            {              
                AbstractUnit unit = factoryMethod(initX, initY, attackTypeManager, mapAllies, renderer,this);
                unit.animationName = unitType.ToLower();
                
                return unit;//factoryMethod(initX,initY,attackTypeManager,map,renderer); // Create the unit using the cached factory method
            }
            else
            {
                throw new ArgumentException("Invalid unit type");
            }
        }


        public async void StartGameLoop()
        {
            
            while (true)
            {              
                stopwatch.Start();

                //await renderer.Clear();//

                //await renderer.Clear(GameObjectsToYSort);

                //unitList.Sort();

                enemySpawner.update(0.016f);

                GameObjectsToYSort.Sort((a, b) => a.SortingCompare(b));


                for (int i = 0; i < GameObjectsToYSort.Count;i++)
                {
                    if (GameObjectsToYSort[i] is AbstractBase absBase)
                    {
                        absBase.Draw(0.016f);
                        absBase.Update();

                    }
                    if (GameObjectsToYSort[i] is AbstractUnit unit)
                    {
                        //await unit.Draw(0.016f);
                        unit.Draw(0.016f);
                        unit.Update(); // here looking for enemies to attack


                    }
                    
                }
                

                for (int i=0;i<projectilesList.Count;i++)
                {
                    projectilesList[i].Update();
                    projectilesList[i].Draw(0.016f);
                }

                for (int i = 0; i < UiElementsList.Count; i++)
                {
                    UiElementsList[i].Update();
                    UiElementsList[i].Draw(0.016f);
                }

                
                ExecutePostFrameActions();

                //await renderer.FinalDraw();
                
                await renderer.DrawAll();
                

                manaManager.Update();

                stopwatch.Stop();

                //await Task.Delay((int)(16.67));

                if (stopwatch.Elapsed.TotalMilliseconds <= 16.67)
                {
                    await Task.Delay((int)(16.67 - stopwatch.Elapsed.TotalMilliseconds)); // Delay for the remaining time
                }
                else
                {
                    Console.Out.WriteLine("took :"+ stopwatch.Elapsed.TotalMilliseconds+" ms");
                }

                stopwatch.Reset();
            }
        }
        public void Death(Projectile projectile)
        {
            projectilesList.Remove(projectile);
            //GameObjectsToYSort.Remove(unit);
        }
        public void Death(AbstractUnit unit)
        {
            unitList.Remove(unit);
            GameObjectsToYSort.Remove(unit);
        }

        public void Death(AbstractBase abstractBase)
        {
            BaseList.Remove(abstractBase);
            GameObjectsToYSort.Remove(abstractBase);
        }


        // Execute all post-frame actions, here executing all knockbacks or moves
        private void ExecutePostFrameActions()
        {
            while (PostFrameActions.Count > 0)
            {
                var action = PostFrameActions.Dequeue();
                action?.Invoke();  // Safely invoke the action
            }

            for (int i =0;i < ConstantPostFrameActions.Count;i++)
            {
                var action = ConstantPostFrameActions[i];
                action?.Invoke();
            }
        }

    }
}
