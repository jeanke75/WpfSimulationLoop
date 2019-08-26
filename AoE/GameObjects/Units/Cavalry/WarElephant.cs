using System.Windows;

namespace AoE.GameObjects.Units.Cavalry
{
    class WarElephant : BaseUnit
    {
        public WarElephant(Vector position, Player owner) : base(position, 100f, 91f, "War Elephant", 450, 15, 0, 0.5f, 2.03f, 1, 2, 0.6f, 4, "WarElephant.png", owner)
        {
            AttackBonuses.Add(ArmorType.Building, 7);
            AttackBonuses.Add(ArmorType.StoneDefense, 7);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
            ArmorTypes.Add(ArmorType.WarElephant, 0);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}