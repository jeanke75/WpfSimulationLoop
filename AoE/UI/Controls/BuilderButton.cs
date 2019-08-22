using AoE.GameObjects.Buildings;

namespace AoE.UI.Controls
{
    class BuilderButton : Button
    {
        public readonly IConstructable Constructable;
        public BuilderButton(double x, double y, double width, double height, string imageId, IConstructable constructable) : base(x, y, width, height, imageId)
        {
            Constructable = constructable;
        }
    }
}