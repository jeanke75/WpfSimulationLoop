using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings.Storage
{
    class LumberCamp : BaseBuilding, IStorage
    {
        public LumberCamp(int x, int y, Player owner) : base(x, y, 2, 2, "Lumber Camp", 35, "LumberCamp.png", owner)
        {
            ConstructionCost.Add(ResourceType.Wood, 100);
        }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Wood;
        }
    }
}