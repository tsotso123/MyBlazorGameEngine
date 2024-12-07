using game.Models.units;

namespace game.Models.interfaces
{
    public interface IMelee
    {
        bool CheckIfEnemiesInMeleeRange(AbstractUnit unit);
        void AttackMelee(AbstractUnit unit);

        public List<AbstractUnit>? PotentialEnemies { get; set; }
    }
}
