using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class Genitour : BaseRangedUnit
    {
        public Genitour(Vector position, Team owner) : base(position, 18f, 18f, "Genitour", 50, 0, 3, 0f, 3.05f, 1, 4, 0.9f, 7f, 0, 3, 1.35f, 5, "Genitour.png", owner)
        {
            AttackBonuses.Add(ArmorType.Archer, 4);
            ArmorTypes.Add(ArmorType.Archer, 0);
            ArmorTypes.Add(ArmorType.CavalryArcher, 1);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}