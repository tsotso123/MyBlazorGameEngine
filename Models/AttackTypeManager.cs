using game.Models.interfaces;
using game.Models.units;
using System.Diagnostics;

namespace game.Models
{
    public class AttackTypeManager : IMelee, IRanged, IProjectile
    {
        MapToGrid mapAllies;
        MapToGrid mapEnemies;

        MapToGrid? map;
        public List<AbstractUnit>? PotentialEnemies { get; set; }
        public AbstractUnit? EnemyToShootAt { get; set; }
        
        public AttackTypeManager(MapToGrid mapAllies, MapToGrid mapEnemies) 
        { 
            this.mapAllies = mapAllies;
            this.mapEnemies = mapEnemies;
        }

        
        public void AttackMelee(AbstractUnit unit)
        {            
            int attack = 0;
            for (int i = 0; i < PotentialEnemies?.Count; i++)
            {
                if (PotentialEnemies[i] != unit && PotentialEnemies[i].AmIEnemy != unit.AmIEnemy)
                {
                    double dist = DistanceBetween2Points(unit.X, unit.Y, PotentialEnemies[i].X, PotentialEnemies[i].Y);
                    if (dist <= unit.Range || (unit.attacking && dist <= unit.UpperAtkRange))
                    {
                        PotentialEnemies[i].Hurt(unit.Damage, unit.attackKnockBack, unit.X, unit.Y, unit.attackKnockSpeed);

                        if (unit.AmIEnemy)
                        {
                            Console.WriteLine(unit.Aoe);
                        }

                        attack++;
                        if (attack >= unit.Aoe)
                        {
                            break;
                        }
                    }
                                           
                   
                }              
            }
        }

        public double DistanceBetween2Points(float x1,float y1,float x2, float y2)
        {
            return Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }



        public bool ProjectileHit(Projectile projectile)
        {           
            int attack = 0;

            if (projectile.AmIEnemy)
            {
                map = mapAllies;
            }
            else
            {
                map = mapEnemies;
            }

            float topLeftCornerX = (float)(projectile.X + 16 - (map.cellSizeWidth * 3) / 2);
            float topLeftCornerY = (float)(projectile.Y - (map.cellSizeHeight * 3) / 2);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {


                    var enemies = map.GetUnitsAt(topLeftCornerX, topLeftCornerY);
                    if (enemies != null && enemies.Count > 0)
                    {
                        for (int k = 0; k < enemies.Count; k++)
                        {
                            

                            if (enemies[k].AmIEnemy != projectile.AmIEnemy)
                            {
                                // to check for all enemies, 
                                // if a move with a speed that is equal to dist, 
                                // would have hit them ? (with the range check)
                                // if so, is the speed higher than dist ? (going to miss?)
                                // if yes, hurt
                                //double dist = DistanceBetween2Points(projectile.X, projectile.Y, PotentialEnemies[i].X, PotentialEnemies[i].Y);
                                //projectile.Speed = (float)dist;
                                //projectile.Move();

                                
                                bool inRange = Math.Abs(Math.Abs(projectile.X) - Math.Abs(enemies[k].X)) <= 32 && Math.Abs(Math.Abs(projectile.Y) - Math.Abs(enemies[k].Y)) <= 32;
                                bool alreadyHit = projectile.AlreadyHitByProjectilesUnits.Contains(enemies[k]);
                                if (inRange && !alreadyHit)
                                {
                                    enemies[k].Hurt(projectile.Damage, projectile.AttackKnockBack, projectile.X, projectile.Y, projectile.AttackKnockSpeed);

                                    projectile.AlreadyHitByProjectilesUnits.Add(enemies[k]);
                                    projectile.UnitsHitCooldownTracker.Add(0);

                                    //projectile.AtkOnCooldown = true;

                                    attack++;
                                    if (attack >= projectile.Aoe)
                                    {
                                        return true;
                                        //break;
                                    }
                                }
                            }
                        }

                    }
                    topLeftCornerX += map.cellSizeWidth;

                }
                topLeftCornerX = (float)(projectile.X + 16 - (map.cellSizeWidth * 3) / 2);

                topLeftCornerY += map.cellSizeHeight;
            }

            

            return false;
        }

        public bool CheckIfEnemiesInMeleeRange(AbstractUnit unit)
        {
            if (unit.AmIEnemy)
            {
                map = mapAllies;
            }
            else
            {
                map = mapEnemies;

            }

            PotentialEnemies = null;

            float topLeftCornerX = (float)(unit.X + 16 - (map.cellSizeWidth * 3) / 2);
            float topLeftCornerY = (float)(unit.Y - (map.cellSizeHeight * 3) / 2);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // depending on unit location, range might exceed map boundries
                    var enemies = map.GetUnitsAt(topLeftCornerX, topLeftCornerY);
                    if (enemies != null && enemies.Count > 0)
                    {
                        for (int k = 0; k < enemies.Count; k++)
                        {
                            if (enemies[k] != unit && enemies[k].AmIEnemy != unit.AmIEnemy)
                            {
                                PotentialEnemies = enemies;
                                double dist = DistanceBetween2Points(unit.X, unit.Y, PotentialEnemies[k].X, PotentialEnemies[k].Y);
                                // range check
                                if (dist <= unit.Range || (unit.attacking && dist <= unit.UpperAtkRange))
                                {
                                    return true;
                                }
                            }
                        }

                    }
                    topLeftCornerX += map.cellSizeWidth;

                }
                topLeftCornerX = (float)(unit.X + 16 - (map.cellSizeWidth * 3) / 2);

                topLeftCornerY += map.cellSizeHeight;
            }

            

            return false;
        }

        // range of a shooting unit, is a square (range X range)
        public bool ScanRange(AbstractUnit unit)
        {
            if (unit.AmIEnemy)
            {
                map = mapAllies;
            }
            else
            {
                map = mapEnemies;
            }
            // note: as Y is lower, point is higher on screen

            float topLeftCornerX = (float)(unit.X+16 - (map.cellSizeWidth * unit.Range) / 2);
            float topLeftCornerY = (float)(unit.Y - (map.cellSizeHeight * unit.Range) / 2);

            for (int i=0; i<unit.Range; i++)
            {
                for (int j = 0; j < unit.Range; j++)
                {                    
                    // depending on unit location, range might exceed map boundries
                    var enemies = map.GetUnitsAt(topLeftCornerX, topLeftCornerY);
                    if (enemies!=null && enemies.Count > 0)
                    {
                        for (int  k = 0; k < enemies.Count; k++)
                        {
                           if (enemies[k] != unit && enemies[k].AmIEnemy!=unit.AmIEnemy)
                           {
                               //PotentialEnemies = enemies;
                               EnemyToShootAt = enemies[k];
                               return true;
                           }
                        }
                        
                    }    
                    topLeftCornerX += map.cellSizeWidth;
                    
                }
                topLeftCornerX = (float)(unit.X+16 - (map.cellSizeWidth * unit.Range)/2);

                topLeftCornerY += map.cellSizeHeight;
            }


            return false;
        }

        //public void shoot(Projectile projectile)
        public void shoot(AbstractUnit unit, Projectile projectile)
        {

            for (int i = 0; i < projectile.Aoe && i < PotentialEnemies!.Count; i++)
            {
                if (PotentialEnemies[i].AmIEnemy != projectile.AmIEnemy)
                {
                    
                }
                
            }
            
        }
    }
}
