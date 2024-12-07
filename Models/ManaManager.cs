using game.Models.interfaces;
using game.Models.units;

namespace game.Models
{
    public class ManaManager
    {
        public float Timer { get; set; } = 0;
        public float TimeForMana { get; set; } = 0.5f;
        public int Mana { get; set; }
        public IGameManager gameManager { get; set; }
        //private MapToGrid mapAllies;
        //private MapToGrid mapEnemies;
        //private MapToGrid? map;
        public ManaManager(IGameManager gameManager) 
        {
            this.gameManager = gameManager;
            //this.mapAllies = mapAllies;
            //this.mapEnemies = mapEnemies;
        }
        public void BuyUnit(AbstractUnit unit)
        {
            if (unit.Cost<=Mana)
            {
                Console.WriteLine("bought unit");
                Mana -= unit.Cost;
                gameManager.AddUnit(unit);
            }
            //else
            //{
            //    if (unit.AmIEnemy)
            //    {
            //        map = mapAllies;
            //    }
            //    else
            //    {
            //        map = mapEnemies;
            //    }
            //    map.RemoveAt(unit.X, unit.Y, unit);
            //}
        }
        public void Update(float elapsedTime=0.016f)
        {
            Timer += elapsedTime;
            if (Timer>=TimeForMana)
            {
                Mana++;
                //Console.WriteLine("mana:"+Mana);
                Timer = 0;
            }
        }
    }
}
