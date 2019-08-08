using System.Windows;

namespace AoE.GameObjects.Units.Siege
{
    class BombardCannon : BaseRangedUnit
    {
        public BombardCannon(Vector position, Team owner) : base(position, 18f, 18f, "Bombard Cannon", 80, 40, 0, 0.5f, 6.5f, 5, 12, 0.92f, 4f, 2, 5, 0.7f, 14, "BombardCannon.png", owner)
        {
            AttackBonuses.Add(ArmorType.Building, 200);
            AttackBonuses.Add(ArmorType.Ship, 40);
            AttackBonuses.Add(ArmorType.StoneDefense, 40);
            AttackBonuses.Add(ArmorType.SiegeWeapon, 20);
            ArmorTypes.Add(ArmorType.SiegeWeapon, 0);
            ArmorTypes.Add(ArmorType.GunpowderUnit, 0);
        }
    }
}