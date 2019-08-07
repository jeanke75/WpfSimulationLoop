using AoE.GameObjects;
using AoE.GameObjects.Units;
using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AoE.Actions
{
    class Attack : BaseAction
    {
        private readonly BaseUnit Unit;
        public readonly BaseUnit Target;
        private readonly List<BaseUnit> AllUnits;

        public Attack(BaseUnit unit, BaseUnit target, List<BaseUnit> units)
        {
            Unit = unit;
            Target = target;
            AllUnits = units;
        }

        public override void Do(float dt)
        {
            if (Target.HitPoints > 0)
            {
                if (Unit.UnitInRange(Target))
                {
                    if (Unit.TimeUntillAttack == 0)
                    {
                        if (Unit.BlastRadius > 0)
                            DealBlastDamage(Target, AllUnits);
                        else
                            DealDamage(Target);
                        Unit.TimeUntillAttack = Unit.RateOfFire;
                    }
                }
                else
                {
                    MoveTowardsAttackRange(dt);
                }
            }
        }

        public override bool Completed()
        {
            return Target.HitPoints == 0;
        }

        private void DealDamage(BaseUnit target)
        {
            if (Unit is BaseRangedUnit rangedUnit && MainWindow.random.NextDouble() > rangedUnit.Accuracy)
                return;

            var meleeDamage = Math.Max(0, Unit.MeleeAttack - target.MeleeArmor);
            var pierceDamage = Math.Max(0, Unit.PierceAttack - target.PierceArmor);
            var bonusDamage = 0;
            foreach (KeyValuePair<ArmorType, int> bonusResist in target.ArmorTypes)
            {
                if (Unit.AttackBonuses.TryGetValue(bonusResist.Key, out int attackBonus))
                {
                    bonusDamage += Math.Max(0, attackBonus - bonusResist.Value);
                }
            }

            target.HitPoints -= Math.Max(1, meleeDamage + pierceDamage + bonusDamage);
            if (!(target.action is Attack || target.action is Move))
                target.action = new Attack(target, Unit, AllUnits);
        }

        private void DealBlastDamage(BaseUnit target, List<BaseUnit> units)
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
                if (distanceToCenter <= unit.BlastRadius * MainWindow.tilesize)
                {
                    unitsInBlast.Add(unit);
                }
            }

            return unitsInBlast;
        }

        protected virtual void MoveTowardsAttackRange(float dt)
        {
            if (Unit is BaseRangedUnit rangedUnit)
            {
                var distance = rangedUnit.DistanceToUnit(Target);
                if (distance > rangedUnit.MaxRange * MainWindow.tilesize)
                {
                    rangedUnit.Position = rangedUnit.Position.MoveTowards(Target.Position, dt, rangedUnit.Speed * MainWindow.tilesize);
                }
                else if (distance < rangedUnit.MinRange * MainWindow.tilesize)
                {
                    // If the target of this unit is not targetting this unit, move away from the target to attack it
                    if (Target.action is Attack targetAttackAction && targetAttackAction.Target != rangedUnit)
                    {
                        var direction = Target.Position - rangedUnit.Position;
                        direction.SetMagnitude(rangedUnit.MinRange * MainWindow.tilesize + Target.Radius + rangedUnit.Radius);
                        var newPosition = rangedUnit.Position - direction;
                        rangedUnit.Position = rangedUnit.Position.MoveTowards(newPosition, dt, rangedUnit.Speed * MainWindow.tilesize);
                    }
                }
            }
            else
            {
                Unit.Position = Unit.Position.MoveTowards(Target.Position, dt, Unit.Speed * MainWindow.tilesize);
            }
        }
    }
}