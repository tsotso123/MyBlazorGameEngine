namespace game.Models
{
    public class EnemyWave
    {
        public float time;
        public int numOfUnits;
        public string? unitToSpawn;
        public EnemyWave(float time,int numOfUnits,string unitToSpawn)
        {
            
            this.time = time;
            this.numOfUnits = numOfUnits;
            this.unitToSpawn = unitToSpawn;
        }
    }
}
