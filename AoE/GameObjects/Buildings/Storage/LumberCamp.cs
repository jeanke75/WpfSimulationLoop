using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings.Storage
{
    class LumberCamp : BaseBuilding, IStorage
    {
        public LumberCamp(int x, int y, Team owner) : base(x, y, 2, 2, "Lumber Camp", "LumberCamp.png", owner) { }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Wood;
        }
    }
}