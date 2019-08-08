using System.Windows;

namespace AoE.GameObjects.Units.Siege
{
    class Mangonel : BaseRangedUnit
    {
        public Mangonel(Vector position, Team owner) : base(position, 30f, 30f, "Mangonel", 50, 40, 0, 1f, 6f, 3, 7, 1f, 3.5f, 0, 6, 0.6f, 9, "Mangonel.png", owner)
        {
            AttackBonuses.Add(ArmorType.Building, 35);
            AttackBonuses.Add(ArmorType.SiegeWeapon, 12);
            ArmorTypes.Add(ArmorType.SiegeWeapon, 0);
        }
    }
}