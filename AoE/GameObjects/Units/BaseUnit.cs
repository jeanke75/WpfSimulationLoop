﻿using AoE.Actions;
using DrawingBase;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AoE.GameObjects.Units
{
    abstract class BaseUnit : BaseGameObject, ISelectable, IOwnable
    {
        public readonly int HitPointsMax;
        private int _HitPoints;
        public int HitPoints
        {
            get
            {
                return _HitPoints;
            }
            set
            {
                if (value < 0)
                    _HitPoints = 0;
                else if (value > HitPointsMax)
                    _HitPoints = HitPointsMax;
                else
                    _HitPoints = value;
            }
        }
        public readonly int MeleeAttack;
        public readonly int PierceAttack;
        public readonly Dictionary<ArmorType, int> AttackBonuses;
        public readonly float BlastRadius;
        public readonly float RateOfFire;
        private float _TimeUntillAttack;
        public float TimeUntillAttack
        {
            get
            {
                return _TimeUntillAttack;
            }
            set
            {
                _TimeUntillAttack = value >= 0 ? value : 0;
            }
        }
        public readonly int MeleeArmor;
        public readonly int PierceArmor;
        public readonly Dictionary<ArmorType, int> ArmorTypes;
        public readonly float Speed;
        public readonly int LineOfSight;

        public BaseAction action;

        private Team owner;

        public BaseUnit(Vector position, double width, double height, string name, int hitPoints, int meleeAttack, int pierceAttack, float blastRadius, float rateOfFire, int meleeArmor, int pierceArmor, float speed, int lineOfSight, string imageId, Team owner) : base(position, width, height, name, "Units/" + imageId)
        {
            HitPointsMax = hitPoints >= 1 ? hitPoints : 1;
            HitPoints = HitPointsMax;
            MeleeAttack = meleeAttack >= 0 ? meleeAttack : 0;
            PierceAttack = pierceAttack >= 0 ? pierceAttack : 0;
            AttackBonuses = new Dictionary<ArmorType, int>();
            BlastRadius = blastRadius >= 0 ? blastRadius : 0;
            RateOfFire = rateOfFire >= 0 ? rateOfFire : 0;
            TimeUntillAttack = 0;
            MeleeArmor = meleeArmor;
            PierceArmor = pierceArmor;
            ArmorTypes = new Dictionary<ArmorType, int>();
            Speed = speed >= 0 ? speed : 0;
            LineOfSight = lineOfSight >= 1 ? lineOfSight : 1;

            this.owner = owner;
        }

        public virtual void Update(float dt, List<BaseUnit> units)
        {
            if (HitPoints > 0)
            {
                TimeUntillAttack -= dt;
                if (TimeUntillAttack < 0)
                    TimeUntillAttack = 0;
                
                if (action != null)
                {
                    if (!action.Completed())
                    {
                        action.Do(dt);
                    }
                    else
                    {
                        action = null;
                    }
                }
                else
                {
                    var enemyUnit = GetClosestUnitInLineOfSight(units);
                    if (enemyUnit != null)
                    {
                        action = new Attack(this, enemyUnit, units);
                    }
                }
            }
        }

        public virtual void Draw(DrawingContext dc, List<Team> teams)
        {
            base.Draw(dc);

            var unitRect = Rect;

            // Draw team color
            var teamColor = teams.Where(x => x.Id == owner.Id).Single().Color;
            dc.PushTransform(new TranslateTransform(unitRect.X + Width / 2, unitRect.Y - 15));
            dc.PushTransform(new RotateTransform(60));
            dc.DrawEquilateralTriangle(new SolidColorBrush(teamColor), null, 8, true);
            dc.Pop();
            dc.Pop();

            // Draw blast radius
            if (MainWindow.ShowAttackRange && action is Attack attackAction && attackAction.Target != null && BlastRadius > 0)
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(attackAction.Target.Position.X, attackAction.Target.Position.Y), BlastRadius * MainWindow.tilesize, BlastRadius * MainWindow.tilesize);

            // Draw line of sight
            if (MainWindow.ShowLineOfSight)
                dc.DrawEllipse(null, new Pen(Brushes.Yellow, 1), new Point(Position.X, Position.Y), Radius + LineOfSight * MainWindow.tilesize, Radius + LineOfSight * MainWindow.tilesize);

            // Draw health
            dc.DrawRectangle(Brushes.Red, null, new Rect(unitRect.X, unitRect.Y - 10, HitPoints / (float)HitPointsMax * Width, 5));

            // Draw time untill next attack
            dc.DrawRectangle(Brushes.SandyBrown, null, new Rect(unitRect.X, unitRect.Y - 5, TimeUntillAttack / RateOfFire * Width, 5));
        }

        private BaseUnit GetClosestUnitInLineOfSight(List<BaseUnit> units)
        {
            BaseUnit closestUnit = null;
            var distanceToClosest = double.MaxValue;
            foreach (BaseUnit unit in units)
            {
                if (unit.owner.Id == owner.Id || unit.HitPoints == 0) continue;
                var distance = Distance(unit) / MainWindow.tilesize;
                if (distance <= LineOfSight + Radius && distance < distanceToClosest)
                {
                    distanceToClosest = distance;
                    closestUnit = unit;
                }
            }
            return closestUnit;
        }

        public virtual bool UnitInRange(BaseUnit other)
        {
            return Distance(other) <= 0.1f * MainWindow.tilesize;
        }

        protected virtual void MoveTowardsPosition(float dt, Vector position)
        {
            Position = Position.MoveTowards(position, dt, Speed * MainWindow.tilesize);
        }

        public Team GetOwner()
        {
            return owner;
        }

        public void SetOwner(Team newOwner)
        {
            owner = newOwner;
        }
    }
}