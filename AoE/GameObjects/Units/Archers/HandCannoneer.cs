﻿using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class HandCannoneer : BaseRangedUnit
    {
        public HandCannoneer(Vector position, Player owner) : base(position, 34f, 45f, "Hand Cannoneer", 35, 0, 17, 0f, 3.49f, 0, 7, 0.65f, 5.5f, 1, 0, 0.96f, 9, "HandCannoneer.png", owner)
        {
            AttackBonuses.Add(ArmorType.Infantry, 10);
            AttackBonuses.Add(ArmorType.Ram, 2);
            AttackBonuses.Add(ArmorType.Spearman, 1);
            ArmorTypes.Add(ArmorType.Archer, 0);
            ArmorTypes.Add(ArmorType.GunpowderUnit, 0);
        }
    }
}