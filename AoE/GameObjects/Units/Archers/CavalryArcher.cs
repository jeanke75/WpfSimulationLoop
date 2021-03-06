﻿using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class CavalryArcher : BaseRangedUnit
    {
        public CavalryArcher(Vector position, Player owner) : base(position, 59f, 78f, "Cavalry Archer", 50, 0, 6, 0f, 2.03f, 0, 4, 0.5f, 7f, 0, 0, 1.4f, 5, "CavalryArcher.png", owner)
        {
            AttackBonuses.Add(ArmorType.Spearman, 2);
            ArmorTypes.Add(ArmorType.Archer, 0);
            ArmorTypes.Add(ArmorType.CavalryArcher, 0);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
        }
    }
}