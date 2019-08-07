using AoE.Actions;
using AoE.GameObjects;
using AoE.GameObjects.Units;
using AoE.GameObjects.Units.Archers;
using AoE.GameObjects.Units.Cavalry;
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

            for (int i = 0; i < 1; i++)
            {
                units.Add(new Knight(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), teams[0]));
            }
            for (int i = 0; i < 1; i++)
            {
                units.Add(new Genitour(new Vector((random.NextDouble() + 7) * tilesize, 200 + i * 30), teams[1]));
            }
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (InputHelper.Mouse.GetState(MouseButton.Left) == ButtonState.Pressed)
            {
                selectedGameObject = null;
                var mousePos = InputHelper.Mouse.GetPosition();
                foreach (BaseUnit unit in units)
                {
                    if (unit is ISelectable selectableUnit && unit.MouseOver(mousePos))
                    {
                        selectedGameObject = selectableUnit;
                        break;
                    }
                }
            }
            else if (InputHelper.Mouse.GetState(MouseButton.Right) == ButtonState.Pressed && selectedGameObject is BaseUnit selectedBaseUnit)
            {
                BaseUnit targetGameObject = null;
                var mousePos = InputHelper.Mouse.GetPosition();
                foreach (BaseUnit unit in units)
                {
                    if (unit.MouseOver(mousePos))
                    {
                        targetGameObject = unit;
                        break;
                    }
                }

                if (targetGameObject != null)
                {
                    if (targetGameObject != selectedBaseUnit)
                    {
                        if (targetGameObject.Team != selectedBaseUnit.Team)
                        {
                            selectedBaseUnit.action = new Attack(selectedBaseUnit, targetGameObject, units);
                        }
                    }
                }
                else
                {
                    selectedBaseUnit.action = new Move(selectedBaseUnit, new Vector(mousePos.X, mousePos.Y));
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
    }
}