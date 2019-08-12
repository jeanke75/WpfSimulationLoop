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
        private readonly ICombat attacker;
        public readonly IDestroyable Target;
        private readonly List<BaseUnit> AllUnits;

        public Attack(ICombat attacker, IDestroyable target, List<BaseUnit> units)
        {
            this.attacker = attacker;
            Target = target;
            AllUnits = units;
        }

        public override void Do(float dt)
        {
            if (!Target.Destroyed())
            {
                if (TargetInRange())
                {
                    if (attacker.GetTimeUntillNextAttack() == 0)
                    {
                        if (attacker.GetBlastRadius() > 0)
                            DealBlastDamage(Target, AllUnits);
                        else
                            DealDamage(Target);
                        attacker.ResetTimeUntillNextAttack();
                    }
                }
                else if (attacker is IMoveable)
                {
                    MoveTowardsAttackRange(dt);
                }
            }
        }

        public override bool Completed()
        {
            return Target.Destroyed();
        }

        private bool TargetInRange()
        {
            var distance = (attacker as BaseGameObject).Distance(Target as BaseGameObject);
            if (attacker is IRangedCombat rangedAttacker)
                return distance >= rangedAttacker.GetAttackRangeMin() * MainWindow.tilesize && distance <= rangedAttacker.GetAttackRangeMax() * MainWindow.tilesize;
            else
                return distance <= 0.1f * MainWindow.tilesize;
        }

        private void DealDamage(IDestroyable target)
        {
            if (attacker is IRangedCombat rangedCombat && MainWindow.random.NextDouble() > rangedCombat.GetAccuracy())
                return;

            // Check if the target can defend itself
            if (target is ICombat combatTarget)
            {
                var meleeDamage = Math.Max(0, attacker.GetMeleeAttack() - combatTarget.GetMeleeArmor());
                var pierceDamage = Math.Max(0, attacker.GetPierceAttack() - combatTarget.GetPierceArmor());
                var bonusDamage = 0;
                foreach (KeyValuePair<ArmorType, int> bonusResist in combatTarget.GetArmorTypes())
                {
                    if (attacker.GetAttackBonuses().TryGetValue(bonusResist.Key, out int attackBonus))
                    {
                        bonusDamage += Math.Max(0, attackBonus - bonusResist.Value);
                    }
                }

                target.TakeDamage(Math.Max(1, meleeDamage + pierceDamage + bonusDamage));

                if (target is IActionable actionableTarget)
                {
                    if (!(actionableTarget.GetAction() is Attack || actionableTarget.GetAction() is Move))
                        combatTarget.Attack(attacker, AllUnits);
                }
            }
            else
            {
                target.TakeDamage(Math.Max(1, attacker.GetMeleeAttack() + attacker.GetPierceAttack()));
            }
        }

        private void DealBlastDamage(IDestroyable target, List<BaseUnit> units)
        {
            var unitsInBlast = GetUnitsInBlastRadius((target as BaseGameObject).Position, units);
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
                if (distanceToCenter <= unit.GetBlastRadius() * MainWindow.tilesize)
                {
                    unitsInBlast.Add(unit);
                }
            }

            return unitsInBlast;
        }

        protected virtual void MoveTowardsAttackRange(float dt)
        {
            BaseGameObject attackerObject = attacker as BaseGameObject;
            BaseGameObject targetObject = Target as BaseGameObject;
            if (attacker is IRangedCombat rangedAttacker)
            {
                var distance = attackerObject.Distance(targetObject);
                if (distance > rangedAttacker.GetAttackRangeMax() * MainWindow.tilesize)
                {
                    attackerObject.Position = attackerObject.Position.MoveTowards(targetObject.Position, dt, (attacker as IMoveable).GetMovementSpeed() * MainWindow.tilesize);
                }
                else if (distance < rangedAttacker.GetAttackRangeMin() * MainWindow.tilesize)
                {
                    // if the attacker is not targetted by it's target, move away from the target to attack
                    if (Target is IActionable actionableTarget && actionableTarget.GetAction() is Attack targetAttackAction && targetAttackAction.Target != rangedAttacker)
                    {
                        var direction = targetObject.Position - attackerObject.Position;
                        direction.SetMagnitude(rangedAttacker.GetAttackRangeMin() * MainWindow.tilesize + targetObject.Radius + attackerObject.Radius);
                        var newPosition = attackerObject.Position - direction;
                        attackerObject.Position = attackerObject.Position.MoveTowards(newPosition, dt, (attacker as IMoveable).GetMovementSpeed() * MainWindow.tilesize);
                    }
                }
            }
            else
            {
                attackerObject.Position = attackerObject.Position.MoveTowards(targetObject.Position, dt, (attacker as IMoveable).GetMovementSpeed() * MainWindow.tilesize);
            }
        }
    }
}