using AoE.GameObjects.Resources;
using System.Collections.Generic;

namespace AoE.GameObjects.Buildings
{
    interface IConstructable
    {
        Dictionary<ResourceType, int> GetConstructionCost();
        float GetConstructionTimeTotal();
        float GetConstructionTime();
        void SetConstructionTime(float time);
        bool GetMultipleBuildersCheck();
        void SetMultipleBuildersCheck(bool check);
    }
}