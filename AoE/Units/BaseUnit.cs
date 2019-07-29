using DrawingBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoE
{
    enum ArmorType
    {
        None,
        Archer,
        Building,
        Camel,
        Castle,
        CavalryArcher,
        Cavalry,
        Condotierro,
        EagleWarrior,
        GunpowderUnit,
        Infantry,
        Monk,
        Ram,
        Ship,
        SiegeWeapon,
        StandardBuilding,
        StoneDefense,
        Spearman,
        UniqueUnit,
        WallAndGate,
        WarElephant
    }

    abstract class BaseUnit
    {
        public Vector Position { get; private set; }
        public readonly float Width;
        public readonly float Height;
        public readonly float Radius;
        public readonly string Name;
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
        protected float TimeUntillAttack { get; set; }
        public readonly int MeleeArmor;
        public readonly int PierceArmor;
        public readonly Dictionary<ArmorType, int> ArmorTypes;
        public readonly float Speed;
        public readonly int LineOfSight;
        public BaseUnit Target { get; set; }

        private readonly ImageSource imageSource;

        public Team Team { get; set; }

        public BaseUnit(Vector position, float width, float height, string name, int hitPoints, int meleeAttack, int pierceAttack, float blastRadius, float rateOfFire, int meleeArmor, int pierceArmor, float speed, int lineOfSight, string imageId, Team team)
        {
            Position = position;
            Width = width;
            Height = height;
            Radius = width < height ? width / 2f : height / 2f;
            Name = name;
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

            imageSource = GetImageSource(imageId);

            Team = team;
        }

        public virtual void Update(float dt, List<BaseUnit> units)
        {
            if (HitPoints > 0)
            {
                TimeUntillAttack -= dt;
                if (TimeUntillAttack < 0)
                    TimeUntillAttack = 0;

                Target = Target ?? GetClosestUnitInLineOfSight(units);

                if (Target != null)
                {
                    if (Target.HitPoints > 0)
                    {
                        if (UnitInRange(Target))
                        {
                            if (TimeUntillAttack == 0)
                            {
                                if (BlastRadius > 0)
                                    DealBlastDamage(Target, units);
                                else
                                    DealDamage(Target);
                                TimeUntillAttack = RateOfFire;
                            }
                        }
                        else
                        {
                            MoveTowardsPosition(dt, Target.Position);
                        }
                    }
                    else
                    {
                        Target = null;
                    }
                }
            }
        }

        public virtual void Draw(DrawingContext dc, List<Team> teams)
        {
            var unitRect = new Rect(Position.X - Width / 2f, Position.Y - Height / 2f, Width, Height);
            var teamColor = teams.Where(x => x.Id == Team.Id).Single().Color;
            // Draw character (TODO draw teamcolor again)
            dc.DrawImage(imageSource, unitRect);
            //dc.DrawEllipse(null, new Pen(Brushes.White, 1), new Point(Position.X, Position.Y), Radius, Radius);

            // Draw blast radius
            if (MainWindow.ShowAttackRange && Target != null && BlastRadius > 0)
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(Target.Position.X, Target.Position.Y), BlastRadius * MainWindow.tilesize, BlastRadius * MainWindow.tilesize);

            // Draw line of sight
            if (MainWindow.ShowLineOfSight)
                dc.DrawEllipse(null, new Pen(Brushes.Yellow, 1), new Point(Position.X, Position.Y), LineOfSight * MainWindow.tilesize, LineOfSight * MainWindow.tilesize);

            // Draw health
            dc.DrawRectangle(Brushes.Red, null, new Rect(unitRect.X, unitRect.Y - 10, HitPoints / (float)HitPointsMax * Width, 5));

            // Draw time untill next attack
            dc.DrawRectangle(Brushes.SandyBrown, null, new Rect(unitRect.X, unitRect.Y - 5, TimeUntillAttack / RateOfFire * Width, 5));
        }

        private ImageSource GetImageSource(string imageId)
        {
            return BitmapDecoder.Create(new Uri("pack://application:,,,/Images/" + imageId), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
        }

        private BaseUnit GetClosestUnitInLineOfSight(List<BaseUnit> units)
        {
            BaseUnit closestUnit = null;
            var distanceToClosest = double.MaxValue;
            foreach (BaseUnit unit in units)
            {
                if (unit.Team.Id == Team.Id || unit.HitPoints == 0) continue;
                var distance = DistanceToUnit(unit) / MainWindow.tilesize;
                if (distance <= LineOfSight && distance < distanceToClosest)
                {
                    distanceToClosest = distance;
                    closestUnit = unit;
                }
            }
            return closestUnit;
        }

        /*
         * Value < 0: Units are overlapping
         */
        protected double DistanceToUnit(BaseUnit other)
        {
            return Math.Sqrt(Math.Pow(other.Position.X - Position.X, 2) + Math.Pow(other.Position.Y - Position.Y, 2)) - (other.Radius + Radius);
        }

        protected virtual bool UnitInRange(BaseUnit other)
        {
            return DistanceToUnit(other) <= 0.1f * MainWindow.tilesize;
        }

        protected virtual void DealDamage(BaseUnit target)
        {
            var meleeDamage = Math.Max(0, MeleeAttack - target.MeleeArmor);
            var pierceDamage = Math.Max(0, PierceAttack - target.PierceArmor);
            var bonusDamage = 0;
            foreach (KeyValuePair<ArmorType, int> bonusResist in target.ArmorTypes)
            {
               if (AttackBonuses.TryGetValue(bonusResist.Key, out int attackBonus))
                {
                    bonusDamage += Math.Max(0, attackBonus - bonusResist.Value);
                }
            }

            target.HitPoints -= Math.Max(1, meleeDamage + pierceDamage + bonusDamage);
            if (target.Target == null)
                target.Target = this;
        }

        protected void DealBlastDamage(BaseUnit target, List<BaseUnit> units)
        {
            var unitsInBlast = GetUnitsInBlastRadius(target.Position, units);
            foreach (BaseUnit unit in unitsInBlast)
            {
                DealDamage(unit);
            }
        }

        private List<BaseUnit> GetUnitsInBlastRadius(Vector centerOfBlast, List<BaseUnit> units)
        {
            List<BaseUnit> unitsInBlast = new List<BaseUnit>();
            foreach (BaseUnit unit in units)
            {
                var distanceToCenter = Math.Sqrt(Math.Pow(unit.Position.X - centerOfBlast.X, 2) + Math.Pow(unit.Position.Y - centerOfBlast.Y, 2));
                if (distanceToCenter <= BlastRadius * MainWindow.tilesize)
                {
                    unitsInBlast.Add(unit);
                }
            }

            return unitsInBlast;
        }

        protected virtual void MoveTowardsPosition(float dt, Vector position)
        {
            Position = Position.MoveTowards(position, dt, Speed * MainWindow.tilesize);
        }
    }
}