using System.Windows;

namespace AoE.Units
{
    class Huskarl : BaseUnit
    {
        public Huskarl(Vector position, Team team) : base(position, 18f, 18f, "Huskarl", 60, 10, 0, 0f, 2.03f, 0, 6, 1.05f, 3, "Huskarl.png", team)
        {
            AttackBonuses.Add(ArmorType.Archer, 6);
            AttackBonuses.Add(ArmorType.EagleWarrior, 2);
            AttackBonuses.Add(ArmorType.StandardBuilding, 2);
            ArmorTypes.Add(ArmorType.Infantry, 10);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}