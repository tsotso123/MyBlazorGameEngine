using game.Models.units;
using game.Models.Bases;
namespace game.Models.interfaces
{
    public interface IGameManager
    {
        public void Death(AbstractUnit unit);
        public void Death(AbstractBase abstractBase);
        public void Death(Projectile projectile);

        public Queue<Action> PostFrameActions { get; }
        public List<Action> ConstantPostFrameActions { get; }

        public void AddUnit(AbstractUnit unit);
        public void AddEnemyUnit(AbstractUnit unit);
        public AbstractUnit CreateUnit(string unitType, int initX, int initY);
        public void AddProjectile(Projectile projectile);
        public int BaseLocationX { get; set; }
        public int BaseLocationY { get; set; }
        public int EnemyBaseLocationX { get; set; }
        public int EnemyBaseLocationY { get; set; }
    }
}
