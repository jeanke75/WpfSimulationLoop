﻿using System.Windows;

namespace AoE.GameObjects.Units.Infantry
{
    class EagleScout : BaseUnit
    {
        public EagleScout(Vector position, Player owner) : base(position, 59f, 57f, "Eagle Scout", 50, 4, 0, 0f, 2.03f, 0, 2, 1.1f, 6, "EagleScout.png", owner)
        {
            AttackBonuses.Add(ArmorType.Monk, 8);
            AttackBonuses.Add(ArmorType.SiegeWeapon, 3);
            AttackBonuses.Add(ArmorType.Cavalry, 2);
            AttackBonuses.Add(ArmorType.Camel, 1);
            AttackBonuses.Add(ArmorType.Ship, 1);
            ArmorTypes.Add(ArmorType.Infantry, 0);
            ArmorTypes.Add(ArmorType.EagleWarrior, 0);
        }
    }
}