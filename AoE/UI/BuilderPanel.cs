using AoE.GameObjects;
using AoE.GameObjects.Buildings;
using AoE.GameObjects.Buildings.Storage;
using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using AoE.UI.Controls;
using DrawingBase.Input;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE.UI
{
    class BuilderPanel
    {
        private readonly MainWindow window;
        private readonly Vector panelOffset;
        private readonly List<BuilderButton> buttons;
        public IConstructable SelectedConstructable { get; set; }

        public BuilderPanel(MainWindow window)
        {
            this.window = window;

            panelOffset = new Vector(0, window.GetHeight() - 89);

            var xOffset = 7d;
            var yOffset = 7d;

            var buttonSize = 25;

            buttons = new List<BuilderButton>
            {
                new BuilderButton(panelOffset.X + xOffset + 2 * buttonSize, panelOffset.Y + yOffset, buttonSize, buttonSize, "Button_Mill.png", new Mill(0, 0, null)),
                new BuilderButton(panelOffset.X + xOffset, panelOffset.Y + yOffset + buttonSize, buttonSize, buttonSize, "Button_LumberCamp.png", new LumberCamp(0, 0, null)),
                new BuilderButton(panelOffset.X + xOffset + buttonSize, panelOffset.Y + yOffset + buttonSize, buttonSize, buttonSize, "Button_MiningCamp.png", new MiningCamp(0, 0, null))
            };

            SelectedConstructable = null;
        }

        public void Update()
        {
            if (window.selectedGameObject is IOwnable && window.selectedGameObject is IBuilder)
            {
                bool buttonClicked = false;
                foreach (BuilderButton button in buttons)
                {
                    button.Enabled = CanPayConstructableCost(button.Constructable.GetConstructionCost());
                    button.Update();
                    if (button.Clicked())
                    {
                        buttonClicked = true;
                        SelectedConstructable = button.Constructable;
                    }
                }

                if (!buttonClicked && SelectedConstructable != null && InputHelper.Mouse.GetState(MouseButton.Left) == DrawingBase.Input.ButtonState.Pressed)
                {
                    if (BuildRequirementsPassed())
                    {
                        // Pay the resources
                        foreach (KeyValuePair<ResourceType, int> resourceCost in SelectedConstructable.GetConstructionCost())
                            window.player.SetResource(resourceCost.Key, window.player.GetResource(resourceCost.Key) - resourceCost.Value);

                        BaseBuilding building = SelectedConstructable as BaseBuilding;

                        // Set the position
                        building.Position = GetTilePositionFromMouse();

                        // Set the owner
                        building.SetOwner(window.player);

                        // Place the building
                        window.buildings.Add(building);

                        // Tell the selected builder to start working on it
                        IBuilder builder = window.selectedGameObject as IBuilder;
                        builder.Build(SelectedConstructable);

                        // Reset the build option
                        SelectedConstructable = null;
                    }
                }
            }
            else if (SelectedConstructable != null)
            {
                SelectedConstructable = null;
            }
        }

        public void Draw(DrawingContext dc)
        {
            if (window.selectedGameObject is IOwnable && window.selectedGameObject is IBuilder)
            {
                foreach (BuilderButton button in buttons)
                    button.Draw(dc);

                if (SelectedConstructable != null)
                {
                    var gridPos = GetTilePositionFromMouse();
                    BaseGameObject baseGameObject = SelectedConstructable as BaseGameObject;
                    dc.PushTransform(new TranslateTransform(gridPos.X, gridPos.Y));
                    dc.PushOpacity(0.75d);
                    baseGameObject.Draw(dc);
                    dc.Pop();
                    dc.Pop();
                }
            }
        }

        private Vector GetTilePositionFromMouse()
        {
            var mousePos = InputHelper.Mouse.GetPosition();
            double x = (int)(mousePos.X / MainWindow.tilesize) * MainWindow.tilesize;
            double y = (int)(mousePos.Y / MainWindow.tilesize) * MainWindow.tilesize;
            return new Vector(x, y);
        }

        private bool BuildRequirementsPassed()
        {
            return CanPayConstructableCost(SelectedConstructable.GetConstructionCost()); //TODO valid build position check
        }

        private bool CanPayConstructableCost(Dictionary<ResourceType, int> cost)
        {
            foreach (KeyValuePair<ResourceType, int> resourceCost in cost)
            {
                if (window.player.GetResource(resourceCost.Key) < resourceCost.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}