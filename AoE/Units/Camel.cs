using System.Windows;

namespace AoE.Units
{
    class Camel : BaseUnit
    {
        public Camel(Vector position, Team team) : base(position, 18f, 18f, "Camel", 100, 6, 0, 0f, 2.03f, 0, 0, 1.45f, 4, "Camel.png", team)
        {
            AttackBonuses.Add(ArmorType.Cavalry, 9);
            AttackBonuses.Add(ArmorType.Camel, 5);
            AttackBonuses.Add(ArmorType.Ship, 5);
            ArmorTypes.Add(ArmorType.Camel, 0);
        }
    }
}