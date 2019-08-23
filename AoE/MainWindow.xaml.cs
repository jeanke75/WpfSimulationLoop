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
        public static readonly Random Random = new Random();
        public static readonly double TileSize = 16f;
        public static readonly bool ShowLineOfSight = true;
        public static readonly bool ShowAttackRange = true;
        public static readonly bool ShowTimeUntillAttack = true;
        public static readonly bool ShowGameObjectRadius = false;
        internal readonly List<Player> Players = new List<Player>();
        internal readonly List<BaseUnit> Units = new List<BaseUnit>();
        internal readonly List<BaseResource> resources = new List<BaseResource>();
        internal readonly List<BaseBuilding> Buildings = new List<BaseBuilding>();

        internal Player Player;
        internal ISelectable SelectedGameObject = null;

        private UserInterface userInterface;

        public override void Initialize()
        {
            SetResolution(800, 600);

            // UI
            userInterface = new UserInterface(this);

            // Players
            for (uint i = 0; i < 2; i++)
            {
                Players.Add(new Player(i, Color.FromRgb((byte)Random.Next(50, 255), (byte)Random.Next(50, 255), (byte)Random.Next(50, 255))));
            }

            Player = Players[0];

            // Add resources to the map
            resources.Add(new Rocks(new Vector(100, 32)));
            resources.Add(new Rocks(new Vector(116, 32)));
            resources.Add(new Tree(new Vector(132, 32)));
            resources.Add(new Tree(new Vector(148, 32)));
            resources.Add(new GoldOre(new Vector(164, 32)));
            resources.Add(new GoldOre(new Vector(180, 32)));

            // Add buildings to the map
            Buildings.Add(new TownCenter(12, 12, Players[0]));
            Buildings[0].SetConstructionTime(0);
            Buildings.Add(new Mill(25, 15, Players[1]));

            // Add units to the map
            for (int i = 0; i < 2; i++)
            {
                Units.Add(new Villager(new Vector((Random.NextDouble() + 1) * TileSize, 200 + i * 30), Players[0]));
                (Units[i * 3] as IGatherer).Gather(resources[0], resources, Buildings);
                Units.Add(new Villager(new Vector((Random.NextDouble() + 1) * TileSize, 200 + i * 30), Players[0]));
                (Units[i * 3 + 1] as IGatherer).Gather(resources[2], resources, Buildings);
                Units.Add(new Villager(new Vector((Random.NextDouble() + 1) * TileSize, 200 + i * 30), Players[0]));
                (Units[i * 3 + 2] as IGatherer).Gather(resources[4], resources, Buildings);
            }

            Units.Add(new Archer(new Vector((Random.NextDouble() + 1) * TileSize + 30, 200), Players[0]));
            Units.Add(new Mangonel(new Vector(GetWidth() - 100, GetHeight() / 2d), Players[1]));
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            userInterface.Update();

            if (InputHelper.Mouse.GetState(MouseButton.Left) == ButtonState.Pressed)
            {
                if (userInterface.builderPanel.SelectedConstructable == null)
                {
                    SelectedGameObject = null;
                    var mousePos = InputHelper.Mouse.GetPosition();
                    foreach (BaseGameObject gameObject in GetAllGameObjects())
                    {
                        if (gameObject is ISelectable selectableUnit && gameObject.MouseOver(mousePos))
                        {
                            SelectedGameObject = selectableUnit;
                            break;
                        }
                    }
                }
            }
            else if (InputHelper.Mouse.GetState(MouseButton.Right) == ButtonState.Pressed)
            {
                // Right-click actions are only available for game objects owned by the player
                if (SelectedGameObject is IOwnable selectedOwnedGameObject && selectedOwnedGameObject.GetOwner() == Player)
                {
                    if (SelectedGameObject is BaseUnit selectedBaseUnit)
                    {
                        // Reset the selected constructable on the preview
                        userInterface.builderPanel.SelectedConstructable = null;

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
                            if (ownableTarget.GetOwner() == Player)
                            {
                                // Is it an unfinished constructable?
                                if (targetGameObject is IConstructable targetOwnableConstructable && targetOwnableConstructable.GetConstructionTime() > 0)
                                {
                                    // Is the selected unit a builder?
                                    if (SelectedGameObject is IBuilder selectedBuilder)
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
                                    selectedCombatUnit.Attack(targetOwnableDestroyable, Units);
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
                                    selectedCombatUnit.Attack(targetNeutralDestroyable, Units);
                                }
                            }
                            else if (targetGameObject is BaseResource targetBaseResource)
                            {
                                // Is our selected unit a gatherer
                                if (selectedBaseUnit is IGatherer selectedGatherer)
                                    selectedGatherer.Gather(targetBaseResource, resources, Buildings);
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
                    if (SelectedGameObject == resource)
                        SelectedGameObject = null;
                }
            }

            for (int i = Buildings.Count - 1; i >= 0; i--)
            {
                var building = Buildings[i];
                if (building.GetHitPoints() == 0)
                {
                    Buildings.Remove(building);
                    if (SelectedGameObject == building)
                        SelectedGameObject = null;
                }
            }

            for (int i = Units.Count - 1; i >= 0; i--)
            {
                var unit = Units[i];
                if (unit.GetHitPoints() > 0)
                    unit.Update(dt, Units);
                else
                {
                    Units.Remove(unit);
                    if (SelectedGameObject == unit)
                        SelectedGameObject = null;
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
                if (resource == SelectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), resource.Rect);
                }
            }

            // Draw buildings
            foreach (BaseBuilding building in Buildings)
            {
                building.Draw(dc);

                // Draw outline if selected
                if (building == SelectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), building.Rect);
                }
            }

            // Draw units
            foreach (BaseUnit unit in Units)
            {
                unit.Draw(dc, Players);

                // Draw outline if selected
                if (unit == SelectedGameObject)
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
            allGameObjects.AddRange(Units);
            allGameObjects.AddRange(resources);
            allGameObjects.AddRange(Buildings);
            return allGameObjects;
        }
    }
}