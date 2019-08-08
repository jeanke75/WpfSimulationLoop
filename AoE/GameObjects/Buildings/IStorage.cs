using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings
{
    interface IStorage
    {
        bool CanStore(ResourceType type);
    }
}