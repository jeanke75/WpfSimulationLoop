using System.Windows;

namespace AoE.GameObjects.Resources
{
    class Rocks : BaseResource
    {
        public Rocks(Vector position) : base(position, 8, 8, "Rocks", ResourceType.Stone, 125, "Rocks.png") { }
    }
}