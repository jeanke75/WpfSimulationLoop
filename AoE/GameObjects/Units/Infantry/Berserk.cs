using System.Windows;

namespace AoE.GameObjects.Units.Infantry
{
    class Berserk : BaseUnit
    {
        public Berserk(Vector position, Player owner) : base(position, 33f, 44f, "Berserk", 61, 9, 0, 0f, 2.03f, 0, 1, 1.05f, 3, "Berserk.png", owner)
        {
            AttackBonuses.Add(ArmorType.EagleWarrior, 2);
            AttackBonuses.Add(ArmorType.StandardBuilding, 2);
            ArmorTypes.Add(ArmorType.Infantry, 0);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}