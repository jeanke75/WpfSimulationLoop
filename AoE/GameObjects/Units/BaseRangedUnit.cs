using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE.GameObjects.Units
{
    abstract class BaseRangedUnit : BaseUnit, IRangedCombat
    {
        protected readonly int MinRange;
        protected readonly int MaxRange;
        protected readonly float Accuracy;
        protected readonly float ProjectileSpeed;

        // TODO list of projectiles

        public BaseRangedUnit(Vector position, double width, double height, string name, int hitPoints, int meleeAttack, int pierceAttack, float blastRadius, float rateOfFire, int minRange, int maxRange, float accuracy, float projectileSpeed, int meleeArmor, int pierceArmor, float speed, int lineOfSight, string imageId, Player owner) :
            base(position, width, height, name, hitPoints, meleeAttack, pierceAttack, blastRadius, rateOfFire, meleeArmor, pierceArmor, speed, lineOfSight, imageId, owner)
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

        public override void Draw(DrawingContext dc, List<Player> players)
        {
            base.Draw(dc, players);

            // Draw attack range
            if (MainWindow.ShowAttackRange)
            {
                // min range
                if (MinRange > 0)
                    dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(Position.X, Position.Y), Radius + MinRange * MainWindow.TileSize, Radius + MinRange * MainWindow.TileSize);

                // max range
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(Position.X, Position.Y), Radius + MaxRange * MainWindow.TileSize, Radius + MaxRange * MainWindow.TileSize);
            }

            // Draw projectile
        }

        #region IRangedcombat
        public double GetAttackRangeMin()
        {
            return MinRange;
        }

        public double GetAttackRangeMax()
        {
            return MaxRange;
        }

        public float GetAccuracy()
        {
            return Accuracy;
        }

        public float GetProjectileSpeed()
        {
            return ProjectileSpeed;
        }
        #endregion
    }
}