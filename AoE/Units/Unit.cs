using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace AoE
{
    enum DamageType
    {
        Melee,
        Pierce
    }

    enum ArmorType
    {
        None,
        Archer,
        Infantry
    }

    abstract class Unit
    {
        public Vector2 Position { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public int HitPoints { get; private set; }
        public int HitPointsMax { get; private set; }
        public int Attack { get; private set; }
        public DamageType DamageType { get; private set; }
        public float RateOfFire { get; private set; }
        private float TimeUntillAttack;
        public int MeleeArmor { get; private set; }
        public int PierceArmor { get; private set; }
        public ArmorType ArmorType { get; private set; }
        public float Speed { get; private set; }
        public int LineOfSight { get; private set; }
        public Unit Target { get; set; }

        private Color Color;

        public uint TeamId { get; private set; }

        public Unit(Vector2 position, float width, float height, int hitPoints, int attack, DamageType damageType, float rateOfFire, int meleeArmor, int pierceArmor, ArmorType armor, float speed, int lineOfSight, Color color, Team team)
        {
            Position = position;
            Width = width;
            Height = height;
            HitPoints = hitPoints;
            HitPointsMax = hitPoints;
            Attack = attack;
            DamageType = damageType;
            RateOfFire = rateOfFire;
            MeleeArmor = meleeArmor;
            PierceArmor = pierceArmor;
            ArmorType = armor;
            Speed = speed;
            LineOfSight = lineOfSight;

            Color = color;

            TeamId = team.Id;
        }

        public virtual void Update(float dt, double tileSize, List<Unit> units)
        {
            Target = Target ?? GetClosestUnitInLineOfSight(units);

            if (Target != null)
            {
                if (Target.HitPoints > 0)
                {
                    // If target
                }
                else
                {
                    Target = null;
                }
            }
        }

        public virtual void Draw(DrawingContext dc, List<Team> teams)
        {
            var unitRect = new Rect(Position.X - Width / 2f, Position.Y - Height / 2f, Width, Height);
            var teamColor = teams.Where(x => x.Id == TeamId).Single().Color;
            // Draw character
            dc.DrawRectangle(new SolidColorBrush(Color), new Pen(new SolidColorBrush(teamColor), 1), unitRect);

            // Draw health
            dc.DrawRectangle(Brushes.Red, null, new Rect(unitRect.X, unitRect.Y - 10, HitPoints / HitPointsMax * Width, 5));
        }

        private Unit GetClosestUnitInLineOfSight(List<Unit> units)
        {
            Unit closestUnit = null;
            var distanceToClosest = double.MaxValue;
            foreach (Unit unit in units)
            {
                if (unit == this || unit.HitPoints == 0) continue;
                var distance = DistanceToUnit(unit);
                if (distance <= LineOfSight && distance < distanceToClosest)
                {
                    distanceToClosest = distance;
                    closestUnit = unit;
                }
            }
            return closestUnit;
        }

        private double DistanceToUnit(Unit other)
        {
            return Math.Sqrt(Math.Pow(other.Position.X - Position.X, 2) + Math.Pow(other.Position.Y - Position.Y, 2));
        }
    }
}