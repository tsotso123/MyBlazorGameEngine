using game.Models.units;

namespace game.Models.interfaces
{
    public interface IRanged
    {
        bool ScanRange(AbstractUnit unit);
        void shoot(AbstractUnit unit,Projectile projectile);
        public AbstractUnit EnemyToShootAt { get; set; }
        public List<AbstractUnit>? PotentialEnemies { get; set; }
    }
}
