using AoE.GameObjects;
using AoE.GameObjects.Buildings;
using AoE.GameObjects.Buildings.Storage;
using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using AoE.GameObjects.Units.Archers;
using AoE.GameObjects.Units.Civilian;
using AoE.GameObjects.Units.Siege;
using AoE.UI;
using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static readonly Random random = new Random();
        public static readonly double tilesize = 10f;
        public static readonly bool ShowLineOfSight = true;
        public static readonly bool ShowAttackRange = true;
        public static readonly bool ShowTimeUntillAttack = true;
        public static readonly bool ShowGameObjectRadius = false;
        internal readonly List<Player> players = new List<Player>();
        internal readonly List<BaseUnit> units = new List<BaseUnit>();
        internal readonly List<BaseResource> resources = new List<BaseResource>();
        internal readonly List<BaseBuilding> buildings = new List<BaseBuilding>();

        internal Player player;
        internal ISelectable selectedGameObject = null;

        private UserInterface userInterface;

        public override void Initialize()
        {
            SetResolution(800, 600);

            // UI
            userInterface = new UserInterface(this);

            // Players
            for (uint i = 0; i < 2; i++)
            {
                players.Add(new Player(i, Color.FromRgb((byte)random.Next(50, 255), (byte)random.Next(50, 255), (byte)random.Next(50, 255))));
            }

            player = players[0];

            // Add resources to the map
            resources.Add(new Rocks(new Vector(100, 32)));
            resources.Add(new Rocks(new Vector(116, 32)));
            resources.Add(new Tree(new Vector(132, 32)));
            resources.Add(new Tree(new Vector(148, 32)));
            resources.Add(new GoldOre(new Vector(164, 32)));
            resources.Add(new GoldOre(new Vector(180, 32)));

            // Add buildings to the map
            buildings.Add(new LumberCamp(10, 10, players[0]));
            buildings.Add(new MiningCamp(12, 10, players[0]));
            buildings.Add(new Mill(14, 10, players[1]));

            // Add units to the map
            for (int i = 0; i < 2; i++)
            {
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), players[0]));
                (units[i * 3] as IGatherer).Gather(resources[0], resources, buildings);
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), players[0]));
                (units[i * 3 + 1] as IGatherer).Gather(resources[2], resources, buildings);
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), players[0]));
                (units[i * 3 + 2] as IGatherer).Gather(resources[4], resources, buildings);
            }

            units.Add(new Archer(new Vector((random.NextDouble() + 1) * tilesize + 30, 200), players[0]));
            units.Add(new Mangonel(new Vector(GetWidth() - 100, GetHeight() / 2d), players[1]));
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (InputHelper.Mouse.GetState(MouseButton.Left) == ButtonState.Pressed)
            {
                selectedGameObject = null;
                var mousePos = InputHelper.Mouse.GetPosition();
                foreach (BaseGameObject gameObject in GetAllGameObjects())
                {
                    if (gameObject is ISelectable selectableUnit && gameObject.MouseOver(mousePos))
                    {
                        selectedGameObject = selectableUnit;
                        break;
                    }
                }
            }
            else if (InputHelper.Mouse.GetState(MouseButton.Right) == ButtonState.Pressed)
            {
                // Right-click actions are only available for game objects owned by the player
                if (selectedGameObject is IOwnable selectedOwnedGameObject && selectedOwnedGameObject.GetOwner() == player)
                {
                    if (selectedGameObject is BaseUnit selectedBaseUnit)
                    {
                        BaseGameObject targetGameObject = null;
                        var mousePos = InputHelper.Mouse.GetPosition();
                        foreach (BaseGameObject gameObject in GetAllGameObjects())
                        {
                            if (gameObject is ISelectable && gameObject.MouseOver(mousePos))
                            {
                                targetGameObject = gameObject;
                                break;
                            }
                        }

                        // Can it be owned?
                        if (targetGameObject is IOwnable ownableTarget)
                        {
                            // Is it our own?
                            if (ownableTarget.GetOwner() == player)
                            {
                                // Is it an unfinished constructable?
                                if (targetGameObject is IConstructable targetOwnableConstructable && targetOwnableConstructable.GetConstructionTime() > 0)
                                {
                                    // Is the selected unit a builder?
                                    if (selectedGameObject is IBuilder selectedBuilder)
                                    {
                                        selectedBuilder.Build(targetOwnableConstructable);
                                    }
                                }
                            }
                            // Is it an enemy destroyable?
                            else if (targetGameObject is IDestroyable targetOwnableDestroyable)
                            {
                                // Does our selected unit have combat skills?
                                if (selectedBaseUnit is ICombat selectedCombatUnit)
                                {
                                    selectedCombatUnit.Attack(targetOwnableDestroyable, units);
                                }
                            }
                        }
                        else
                        {
                            // Is it a destroyable object?
                            if (targetGameObject is IDestroyable targetNeutralDestroyable)
                            {
                                // Does our selected unit have combat skills?
                                if (selectedBaseUnit is ICombat selectedCombatUnit)
                                {
                                    selectedCombatUnit.Attack(targetNeutralDestroyable, units);
                                }
                            }
                            else if (targetGameObject is BaseResource targetBaseResource)
                            {
                                // Is our selected unit a gatherer
                                if (selectedBaseUnit is IGatherer selectedGatherer)
                                    selectedGatherer.Gather(targetBaseResource, resources, buildings);
                            }
                            else if (targetGameObject == null)
                            {
                                // Is our selected unit moveable?
                                if (selectedBaseUnit is IMoveable selectedMoveable)
                                    selectedMoveable.MoveTo(new Vector(mousePos.X, mousePos.Y));
                            }
                        }
                    }
                }
            }

            for (int i = resources.Count - 1; i >= 0; i--)
            {
                var resource = resources[i];
                if (resource.Amount == 0)
                {
                    resources.Remove(resource);
                    if (selectedGameObject == resource)
                        selectedGameObject = null;
                }
            }

            for (int i = buildings.Count - 1; i >= 0; i--)
            {
                var building = buildings[i];
                if (building.GetHitPoints() == 0)
                {
                    buildings.Remove(building);
                    if (selectedGameObject == building)
                        selectedGameObject = null;
                }
            }

            for (int i = units.Count - 1; i >= 0; i--)
            {
                var unit = units[i];
                if (unit.GetHitPoints() > 0)
                    unit.Update(dt, units);
                else
                {
                    units.Remove(unit);
                    if (selectedGameObject == unit)
                        selectedGameObject = null;
                }
            }
        }

        public override void Draw(DrawingContext dc)
        {
            // Draw resources
            foreach (BaseResource resource in resources)
            {
                resource.Draw(dc);

                // Draw outline if selected
                if (resource == selectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), resource.Rect);
                }
            }

            // Draw buildings
            foreach (BaseBuilding building in buildings)
            {
                building.Draw(dc);

                // Draw outline if selected
                if (building == selectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), building.Rect);
                }
            }

            // Draw units
            foreach (BaseUnit unit in units)
            {
                unit.Draw(dc, players);

                // Draw outline if selected
                if (unit == selectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), unit.Rect);
                }
            }

            // Draw UI
            userInterface.Draw(dc);
        }

        public override void Cleanup()
        {
        }

        private List<BaseGameObject> GetAllGameObjects()
        {
            var allGameObjects = new List<BaseGameObject>();
            allGameObjects.AddRange(units);
            allGameObjects.AddRange(resources);
            allGameObjects.AddRange(buildings);
            return allGameObjects;
        }
    }
}