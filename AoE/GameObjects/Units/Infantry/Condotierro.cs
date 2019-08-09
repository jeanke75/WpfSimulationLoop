using System.Windows;

namespace AoE.GameObjects.Units.Infantry
{
    class Condotierro : BaseUnit
    {
        public Condotierro(Vector position, Player owner) : base(position, 18f, 18f, "Condotierro", 80, 9, 0, 0f, 1.93f, 1, 0, 1.2f, 6, "Condotierro.png", owner)
        {
            AttackBonuses.Add(ArmorType.GunpowderUnit, 10);
            AttackBonuses.Add(ArmorType.StandardBuilding, 2);
            ArmorTypes.Add(ArmorType.Infantry, 10);
            ArmorTypes.Add(ArmorType.UniqueUnit, 0);
            ArmorTypes.Add(ArmorType.Condotierro, 0);
        }
    }
}