using System.Windows;

namespace AoE.GameObjects.Resources
{
    class GoldOre : BaseResource
    {
        public GoldOre(Vector position) : base(position, 64, 64, "Gold Ore", ResourceType.Gold, 125, "GoldOre.png") { }
    }
}