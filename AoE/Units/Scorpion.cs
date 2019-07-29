using System.Windows;

namespace AoE.Units
{
    class Scorpion : BaseRangedUnit
    {
        public Scorpion(Vector position, Team team) : base(position, 36f, 36f, "Scorpion", 40, 0, 12, 0f, 3.6f, 2, 7, 1f, 6f, 0, 7, 0.65f, 9, "Scorpion.png", team)
        {
            AttackBonuses.Add(ArmorType.WarElephant, 6);
            AttackBonuses.Add(ArmorType.Building, 2);
            AttackBonuses.Add(ArmorType.Ram, 2);
            ArmorTypes.Add(ArmorType.SiegeWeapon, 0);
        }
    }
}