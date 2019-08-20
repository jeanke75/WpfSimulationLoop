using AoE.Actions;
using DrawingBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AoE.GameObjects.Units
{
    abstract class BaseUnit : BaseGameObject, ISelectable, IActionable, IMoveable, ICombat, IOwnable
    {
        private readonly int HitPointsMax;
        private int _HitPoints;
        protected int HitPoints
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
        private readonly int MeleeAttack;
        private readonly int PierceAttack;
        protected readonly Dictionary<ArmorType, int> AttackBonuses;
        private readonly float BlastRadius;
        private readonly float RateOfFire;
        private float _TimeUntillAttack;
        protected float TimeUntillAttack
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
        private readonly int MeleeArmor;
        private readonly int PierceArmor;
        protected readonly Dictionary<ArmorType, int> ArmorTypes;
        private readonly float Speed;
        public readonly int LineOfSight;

        protected BaseAction action;

        private Player owner;

        public BaseUnit(Vector position, double width, double height, string name, int hitPoints, int meleeAttack, int pierceAttack, float blastRadius, float rateOfFire, int meleeArmor, int pierceArmor, float speed, int lineOfSight, string imageId, Player owner) : base(position, width, height, name, "Units/" + imageId)
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

        public virtual void Draw(DrawingContext dc, List<Player> players)
        {
            base.Draw(dc);

            var unitRect = Rect;

            // Draw player color
            var playerColor = players.Where(x => x.Id == owner.Id).Single().Color;
            dc.PushTransform(new TranslateTransform(unitRect.X + Width / 2, unitRect.Y - 15));
            dc.PushTransform(new RotateTransform(60));
            dc.DrawEquilateralTriangle(new SolidColorBrush(playerColor), null, 8, true);
            dc.Pop();
            dc.Pop();

            // Draw blast radius
            if (MainWindow.ShowAttackRange && action is Attack attackAction && attackAction.Target != null && BlastRadius > 0)
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point((attackAction.Target as BaseGameObject).Position.X, (attackAction.Target as BaseGameObject).Position.Y), BlastRadius * MainWindow.tilesize, BlastRadius * MainWindow.tilesize);

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
                if (distance <= LineOfSight && distance < distanceToClosest)
                {
                    distanceToClosest = distance;
                    closestUnit = unit;
                }
            }
            return closestUnit;
        }

        #region IActionable
        public BaseAction GetAction()
        {
            return action;
        }
        #endregion

        #region IMoveable
        public float GetMovementSpeed()
        {
            return Speed;
        }

        public void MoveTo(Vector destination)
        {
            action = new Move(this, destination);
        }
        #endregion

        #region ICombat
        public int GetHitPoints()
        {
            return HitPoints;
        }

        public int GetHitPointsMax()
        {
            return HitPointsMax;
        }

        public int GetMeleeAttack()
        {
            return MeleeAttack;
        }

        public int GetPierceAttack()
        {
            return PierceAttack;
        }

        public ReadOnlyDictionary<ArmorType, int> GetAttackBonuses()
        {
            return new ReadOnlyDictionary<ArmorType, int>(AttackBonuses);
        }

        public float GetBlastRadius()
        {
            return BlastRadius;
        }

        public float GetRateOfFire()
        {
            return RateOfFire;
        }

        public float GetTimeUntillNextAttack()
        {
            return TimeUntillAttack;
        }

        public void ResetTimeUntillNextAttack()
        {
            TimeUntillAttack = GetRateOfFire();
        }

        public int GetMeleeArmor()
        {
            return MeleeArmor;
        }

        public int GetPierceArmor()
        {
            return PierceArmor;
        }

        public ReadOnlyDictionary<ArmorType, int> GetArmorTypes()
        {
            return new ReadOnlyDictionary<ArmorType, int>(ArmorTypes);
        }

        public void Attack(IDestroyable destroyable, List<BaseUnit> destroyables)
        {
            action = new Attack(this, destroyable, destroyables);
        }

        public void TakeDamage(int damage)
        {
            HitPoints -= damage;
        }

        public bool Destroyed()
        {
            return HitPoints == 0;
        }
        #endregion

        #region IOwnable
        public Player GetOwner()
        {
            return owner;
        }

        public void SetOwner(Player newOwner)
        {
            owner = newOwner;
        }
        #endregion
    }
}