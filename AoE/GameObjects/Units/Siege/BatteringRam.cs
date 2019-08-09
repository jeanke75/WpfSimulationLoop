using System.Windows;

namespace AoE.GameObjects.Units.Siege
{
    class BatteringRam : BaseUnit
    {
        public BatteringRam(Vector position, Player owner) : base(position, 36f, 36f, "Battering Ram", 175, 2, 0, 0f, 5f, -3, 180, 0.5f, 3, "BatteringRam.png", owner)
        {
            AttackBonuses.Add(ArmorType.Building, 125);
            AttackBonuses.Add(ArmorType.SiegeWeapon, 40);
            ArmorTypes.Add(ArmorType.SiegeWeapon, 0);
            ArmorTypes.Add(ArmorType.Ram, 0);
        }
    }
}