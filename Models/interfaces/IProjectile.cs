namespace game.Models.interfaces
{
    public interface IProjectile
    {
        public bool ProjectileHit(Projectile projectile);
        public double DistanceBetween2Points(float x1, float y1, float x2, float y2);
    }
}
