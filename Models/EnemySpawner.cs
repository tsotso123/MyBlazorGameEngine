using game.Models.interfaces;
using game.Models.units;

namespace game.Models
{
    public class EnemySpawner : IEnemySpawner
    {
        IGameManager gameManager;

        float timer = 0;

        EnemyWave[] level; 

        int iterator = 0;

        Random rand;
        public EnemySpawner(IGameManager gameManager, EnemyWave[] level)
        {
            this.gameManager = gameManager;

            this.level=level;
            rand = new Random();
        }
        public void update(float elapsedTime)
        {
            timer += elapsedTime;
            if (iterator < level.Length && timer >= level[iterator].time)
            {
                int finalIter = iterator;
                gameManager.PostFrameActions.Enqueue(()=>
                    {                                               
                        for (int i = 0; i < level[finalIter].numOfUnits; i++)
                        {
                            // to randomize y when spawned, when walking, and when finding enemies
                            int yer = gameManager.BaseLocationY + 64 - (i + 1) * 2;
                            AbstractUnit unit = gameManager.CreateUnit(level[finalIter].unitToSpawn!, gameManager.EnemyBaseLocationX + 45 -(i+1)*2, gameManager.BaseLocationY + 64  - (i+1)*8);
                            unit.PostMovementActions.Enqueue(()=>
                            {
                                //unit.MoveTo(gameManager.BaseLocationX+100,rand.Next(gameManager.BaseLocationY+80, gameManager.BaseLocationY+120));
                                unit.MoveTo(gameManager.BaseLocationX + 100, unit.Y+16);
                            });
                            gameManager.AddEnemyUnit(unit);
                        }
                        
                    });
                
                
                timer = 0;
                iterator++;               
            }
        }
    }
}
