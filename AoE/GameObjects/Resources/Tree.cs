using System.Windows;

namespace AoE.GameObjects.Resources
{
    class Tree : BaseResource
    {
        public Tree(Vector position) : base(position, 64, 64, "Tree", ResourceType.Wood, 125, "Tree.png") { }
    }
}