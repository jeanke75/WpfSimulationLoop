using System.Windows;

namespace AoE.Units
{
    class Spearman : BaseUnit
    {
        public Spearman(Vector position, Team team) : base(position, 18f, 18f, "Spearman", 45, 3, 0, 0f, 3.05f, 0, 0, 1f, 4, "Spearman.png", team)
        {
            AttackBonuses.Add(ArmorType.Cavalry, 15);
            AttackBonuses.Add(ArmorType.WarElephant, 15);
            AttackBonuses.Add(ArmorType.Camel, 12);
            AttackBonuses.Add(ArmorType.Ship, 9);
            AttackBonuses.Add(ArmorType.EagleWarrior, 1);
            ArmorTypes.Add(ArmorType.Infantry, 0);
            ArmorTypes.Add(ArmorType.Spearman, 0);
        }
    }
}