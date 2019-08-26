using System.Windows;

namespace AoE.GameObjects.Units.Infantry
{
    class Militia : BaseUnit
    {
        public Militia(Vector position, Player owner) : base(position, 28f, 61f, "Militia", 40, 4, 0, 0, 2.03f, 0, 1, 0.9f, 4, "Militia.png", owner)
        {
            ArmorTypes.Add(ArmorType.Infantry, 0);
        }
    }
}