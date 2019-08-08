using AoE.GameObjects.Buildings;
using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using DrawingBase;
using System;
using System.Collections.Generic;

namespace AoE.Actions
{
    class Gather : BaseAction
    {
        private readonly BaseUnit Unit;
        public BaseResource Resource { get; private set; }
        public readonly int AmountCarriedMax;
        public int AmountCarried { get; private set; }
        private readonly double GatherSpeed;
        private double TimeUntillNextGather;

        private readonly List<BaseResource> Resources;
        private readonly List<BaseBuilding> Buildings;

        public Gather(BaseUnit gatherer, BaseResource resource, List<BaseResource> resources, List<BaseBuilding> buildings)
        {
            Unit = gatherer;
            Resource = resource;
            switch (Resource.Type)
            {
                case ResourceType.Food:
                    AmountCarriedMax = 10;
                    GatherSpeed = 1 / 0.319d;
                    break;
                case ResourceType.Gold:
                    AmountCarriedMax = 10;
                    GatherSpeed = 1 / 0.379d;
                    break;
                case ResourceType.Stone:
                    AmountCarriedMax = 10;
                    GatherSpeed = 1 / 0.359d;
                    break;
                case ResourceType.Wood:
                    AmountCarriedMax = 16;
                    GatherSpeed = 1 / 0.388d;
                    break;
                default:
                    throw new Exception("Undefined resourcetype to gather.");
            }
            AmountCarried = 0;
            TimeUntillNextGather = GatherSpeed;

            Resources = resources;
            Buildings = buildings;
        }

        public override void Do(float dt)
        {
            if (!Completed())
            {
                // Check if we still have an active resource to gather and if we are not full
                if (AmountCarried < AmountCarriedMax)
                {
                    // Check if the resource has been depleted
                    if (Resource.Amount > 0)
                    {
                        var distance = Unit.Distance(Resource) / MainWindow.tilesize;
                        if (distance < 0.1f)
                        {
                            TimeUntillNextGather -= dt;
                            if (TimeUntillNextGather <= 0)
                            {
                                AmountCarried += Resource.Gather(1);

                                // Reset timer
                                TimeUntillNextGather = GatherSpeed;
                            }
                        }
                        else
                        {
                            Unit.Position = Unit.Position.MoveTowards(Resource.Position, dt, Unit.Speed * MainWindow.tilesize);

                            // Reset timer
                            TimeUntillNextGather = GatherSpeed;
                        }
                    }
                    else
                    {
                        // Find the resource of the same type closest to the last resource gathered
                        BaseResource closestResource = null;
                        var distanceToClosest = double.MaxValue;
                        foreach (BaseResource nextResource in Resources)
                        {
                            if (nextResource.Type == Resource.Type && nextResource.Amount > 0)
                            {
                                var distance = Resource.Distance(nextResource); // No need to divide by tilesize because we don't need the actual distance

                                if (distance < distanceToClosest && distance <= Unit.LineOfSight + Unit.Radius)
                                {
                                    closestResource = nextResource;
                                    distanceToClosest = distance;
                                }
                            }
                        }

                        if (closestResource != null)
                        {
                            Resource = closestResource;
                        }
                        else if (AmountCarried > 0)
                        {
                            // If no resources are found and there are still resources being carried, store them
                            StoreResources(dt);
                        }
                        else
                        {
                            // No resources found and nothing carried (Job Complete)
                            Resource = null;
                        }

                        // Reset timer
                        TimeUntillNextGather = GatherSpeed;
                    }
                }
                else
                {
                    StoreResources(dt);
                }
            }
        }

        public override bool Completed()
        {
            return Resource == null && AmountCarried == 0;
        }

        private void StoreResources(float dt)
        {
            BaseBuilding closestStorage = null;
            var distanceToClosest = double.MaxValue;
            foreach (BaseBuilding building in Buildings)
            {
                if (building.GetOwner() == Unit.GetOwner() && building is IStorage storage && storage.CanStore(Resource.Type))
                {
                    var distance = Unit.Distance(building) / MainWindow.tilesize;
                    if (distance < distanceToClosest)
                    {
                        closestStorage = building;
                        distanceToClosest = distance;
                    }
                }
            }

            // Check if a storage is available
            if (closestStorage != null)
            {
                if (distanceToClosest < 0.1f)
                {
                    // If storage is within range, drop off goods
                    Team owner = Unit.GetOwner();
                    owner.SetResource(Resource.Type, owner.GetResource(Resource.Type) + AmountCarried);
                    AmountCarried = 0;
                }
                else
                {
                    // Otherwise move towards it
                    Unit.Position = Unit.Position.MoveTowards(closestStorage.Position, dt, Unit.Speed * MainWindow.tilesize);
                }
            }
        }
    }
}