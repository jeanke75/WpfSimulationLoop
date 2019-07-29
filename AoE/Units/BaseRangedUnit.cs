using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE.Units
{
    abstract class BaseRangedUnit : BaseUnit
    {
        public readonly int MinRange;
        public readonly int MaxRange;
        public readonly float Accuracy;
        public readonly float ProjectileSpeed;
        
        // TODO list of projectiles

        public BaseRangedUnit(Vector position, float width, float height, string name, int hitPoints, int meleeAttack, int pierceAttack, float blastRadius, float rateOfFire, int minRange, int maxRange, float accuracy, float projectileSpeed, int meleeArmor, int pierceArmor, float speed, int lineOfSight, string imageId, Team team) :
            base(position, width, height, name, hitPoints, meleeAttack, pierceAttack, blastRadius, rateOfFire, meleeArmor, pierceArmor, speed, lineOfSight, imageId, team)
        {
            MinRange = minRange >= 0 ? minRange : 0;
            if (maxRange >= minRange)
                MaxRange = maxRange >= 1 ? maxRange : 1;
            else
                throw new Exception("Max range smaller than min range!");

            if (accuracy < 0.01f)
                Accuracy = 0.01f;
            else if (accuracy > 1f)
                Accuracy = 1f;
            else
                Accuracy = accuracy;

            ProjectileSpeed = projectileSpeed >= 1f ? projectileSpeed : 1f;
        }

        public override void Draw(DrawingContext dc, List<Team> teams)
        {
            base.Draw(dc, teams);

            // Draw attack range
            if (MainWindow.ShowAttackRange)
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(Position.X, Position.Y), MaxRange * MainWindow.tilesize, MaxRange * MainWindow.tilesize);

            // Draw projectile
        }

        protected override bool UnitInRange(BaseUnit other)
        {
            var distance = DistanceToUnit(other);
            return distance >= MinRange * MainWindow.tilesize && distance <= MaxRange * MainWindow.tilesize;
        }

        protected override void DealDamage(BaseUnit target)
        {
            if (MainWindow.random.NextDouble() <= Accuracy)
            {
                base.DealDamage(target);
            }
        }

        protected override void MoveTowardsPosition(float dt, Vector position)
        {
            base.MoveTowardsPosition(dt, position);
        }
    }
}