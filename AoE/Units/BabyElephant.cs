using System.Windows;

namespace AoE.Units
{
    class BabyElephant : BaseUnit
    {
        public BabyElephant(Vector position, Team team) : base(position, 18f, 18f, "Baby Elephant", 250, 12, 0, 0.4f, 2.03f, 1, 2, 0.85f, 4, "BabyElephant.png", team)
        {
            AttackBonuses.Add(ArmorType.Building, 7);
            AttackBonuses.Add(ArmorType.StoneDefense, 7);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
            ArmorTypes.Add(ArmorType.WarElephant, 0);
        }
    }
}