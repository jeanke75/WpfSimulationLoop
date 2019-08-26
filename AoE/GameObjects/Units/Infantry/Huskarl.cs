using System.Windows;

namespace AoE.GameObjects.Units.Infantry
{
    class Huskarl : BaseUnit
    {
        public Huskarl(Vector position, Player owner) : base(position, 31f, 51f, "Huskarl", 60, 10, 0, 0f, 2.03f, 0, 6, 1.05f, 3, "Huskarl.png", owner)
        {
            AttackBonuses.Add(ArmorType.Archer, 6);
            AttackBonuses.Add(ArmorType.EagleWarrior, 2);
            AttackBonuses.Add(ArmorType.StandardBuilding, 2);
            ArmorTypes.Add(ArmorType.Infantry, 10);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}