using System.Windows;

namespace AoE.GameObjects.Resources
{
    class GoldOre : BaseResource
    {
        public GoldOre(Vector position) : base(position, 8, 8, "Gold Ore", ResourceType.Gold, 125, "GoldOre.png" ) { }
    }
}