using System.Windows;

namespace AoE.GameObjects.Units.Cavalry
{
    class Tarkan : BaseUnit
    {
        public Tarkan(Vector position, Player owner) : base(position, 58f, 83f, "Tarkan", 100, 8, 0, 0f, 2.13f, 1, 3, 1.35f, 5, "Tarkan.png", owner)
        {
            AttackBonuses.Add(ArmorType.StoneDefense, 12);
            AttackBonuses.Add(ArmorType.Castle, 10);
            AttackBonuses.Add(ArmorType.Building, 8);
            AttackBonuses.Add(ArmorType.WallAndGate, 8);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}