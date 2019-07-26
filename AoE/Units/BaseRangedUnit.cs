using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE.Units
{
    abstract class BaseRangedUnit : BaseUnit
    {
        public readonly int Range;
        public readonly float Accuracy;
        public readonly float ProjectileSpeed;
        // TODO list of projectiles

        public BaseRangedUnit(Vector position, float width, float height, int hitPoints, int meleeAttack, int pierceAttack, float rateOfFire, int range, float accuracy, float projectileSpeed, int meleeArmor, int pierceArmor, float speed, int lineOfSight, Color color, Team team) :
            base(position, width, height, hitPoints, meleeAttack, pierceAttack, rateOfFire, meleeArmor, pierceArmor, speed, lineOfSight, color, team)
        {
            Range = range >= 1 ? range : 1;

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
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(Position.X, Position.Y), Range * 10, Range * 10);

            // Draw projectile
        }

        protected override bool UnitInRange(BaseUnit other)
        {
            return DistanceToUnit(other) <= Range * MainWindow.tilesize;
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