using System.Windows;

namespace AoE.Units
{
    class SiegeTower : BaseRangedUnit
    {
        public SiegeTower(Vector position, Team team) : base(position, 18f, 18f, "Siege Tower", 220, 0, 0, 0f, 1.83f, 0, 0, 0f, 0f, -2, 100, 0.8f, 8, "SiegeTower.png", team)
        {
            ArmorTypes.Add(ArmorType.SiegeWeapon, 0);
            ArmorTypes.Add(ArmorType.Ram, 0);
        }
    }
}