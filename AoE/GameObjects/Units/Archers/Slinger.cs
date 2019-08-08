using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class Slinger : BaseRangedUnit
    {
        public Slinger(Vector position, Team owner) : base(position, 18f, 18f, "Slinger", 40, 0, 5, 0f, 2.03f, 1, 5, 0.9f, 5.5f, 0, 0, 0.96f, 7, "Slinger.png", owner)
        {
            AttackBonuses.Add(ArmorType.Infantry, 10);
            AttackBonuses.Add(ArmorType.Condotierro, 10);
            AttackBonuses.Add(ArmorType.Ram, 3);
            AttackBonuses.Add(ArmorType.Spearman, 1);
            ArmorTypes.Add(ArmorType.Archer, 0);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
        }
    }
}