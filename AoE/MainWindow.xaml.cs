using AoE.Actions;
using AoE.GameObjects;
using AoE.GameObjects.Buildings;
using AoE.GameObjects.Buildings.Storage;
using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using AoE.GameObjects.Units.Archers;
using AoE.GameObjects.Units.Cavalry;
using AoE.GameObjects.Units.Civilian;
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
        readonly List<Team> teams = new List<Team>();
        readonly List<BaseUnit> units = new List<BaseUnit>();
        readonly List<BaseResource> resources = new List<BaseResource>();
        readonly List<BaseBuilding> buildings = new List<BaseBuilding>();

        ISelectable selectedGameObject = null;
        private SelectionPanel selectionPanel;

        public override void Initialize()
        {
            SetResolution(800, 600);

            selectionPanel = new SelectionPanel(this);

            for (uint i = 0; i < 2; i++)
            {
                teams.Add(new Team(i, Color.FromRgb((byte)random.Next(50, 255), (byte)random.Next(50, 255), (byte)random.Next(50, 255))));
            }

            // Add resources to the map
            resources.Add(new Rocks(new Vector(100, 4)));
            resources.Add(new Rocks(new Vector(116, 4)));
            resources.Add(new Tree(new Vector(132, 4)));
            resources.Add(new Tree(new Vector(148, 4)));
            resources.Add(new GoldOre(new Vector(164, 4)));
            resources.Add(new GoldOre(new Vector(180, 4)));

            buildings.Add(new LumberCamp(10, 10, teams[0]));
            buildings.Add(new MiningCamp(12, 10, teams[0]));
            buildings.Add(new Mill(14, 10, teams[0]));

            for (int i = 0; i < 2; i++)
            {
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), teams[0]));
                units[i * 3].action = new Gather(units[i * 3], resources[0], resources, buildings);
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), teams[0]));
                units[i * 3 + 1].action = new Gather(units[i * 3 + 1], resources[2], resources, buildings);
                units.Add(new Villager(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), teams[0]));
                units[i * 3 + 2].action = new Gather(units[i * 3 + 2], resources[4], resources, buildings);
            }
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

                    if (targetGameObject is BaseUnit targetBaseUnit)
                    {
                        if (targetBaseUnit != selectedBaseUnit)
                        {
                            if (targetBaseUnit.GetOwner() != selectedBaseUnit.GetOwner())
                            {
                                selectedBaseUnit.action = new Attack(selectedBaseUnit, targetBaseUnit, units);
                            }
                        }
                    }
                    else if (targetGameObject is BaseResource targetBaseResource)
                    {
                        if (selectedBaseUnit is Villager)
                            selectedBaseUnit.action = new Gather(selectedBaseUnit, targetBaseResource, resources, buildings);
                    }
                    else
                    {
                        selectedBaseUnit.action = new Move(selectedBaseUnit, new Vector(mousePos.X, mousePos.Y));
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

            for (int i = units.Count - 1; i >= 0; i--)
            {
                var unit = units[i];
                if (unit.HitPoints > 0)
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
                unit.Draw(dc, teams);

                // Draw outline if selected
                if (unit == selectedGameObject)
                {
                    dc.DrawRectangle(null, new Pen(Brushes.White, 1), unit.Rect);
                }
            }

            // Draw UI
            selectionPanel.Draw(dc, selectedGameObject);
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