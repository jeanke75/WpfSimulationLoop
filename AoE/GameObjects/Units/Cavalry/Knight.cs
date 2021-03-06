﻿using System.Windows;

namespace AoE.GameObjects.Units.Cavalry
{
    class Knight : BaseUnit
    {
        public Knight(Vector position, Player owner) : base(position, 56f, 85f, "Knight", 100, 10, 0, 0f, 1.83f, 2, 2, 1.35f, 4, "Knight.png", owner)
        {
            ArmorTypes.Add(ArmorType.Cavalry, 0);
        }
    }
}