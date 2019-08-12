using AoE.GameObjects.Buildings;
using AoE.GameObjects.Resources;
using System.Collections.Generic;

namespace AoE.GameObjects.Units
{
    interface IGatherer : IActionable
    {
        void Gather(BaseResource resource, List<BaseResource> resources, List<BaseBuilding> buildings);
    }
}