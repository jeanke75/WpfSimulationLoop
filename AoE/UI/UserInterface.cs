using System.Windows;
using System.Windows.Media;

namespace AoE.UI
{
    class UserInterface
    {
        private readonly MainWindow window;

        private readonly ImageSource imageSource;
        private readonly Rect rect;

        private readonly PlayerInfoPanel playerInfoPanel;
        internal readonly BuilderPanel builderPanel;
        private readonly SelectionPanel selectionPanel;

        public UserInterface(MainWindow window)
        {
            this.window = window;

            imageSource = Global.GetImageSource("UI/UserInterface.png");

            rect = new Rect(0, 0, window.GetWidth(), window.GetHeight());
            playerInfoPanel = new PlayerInfoPanel(window);
            builderPanel = new BuilderPanel(window);
            selectionPanel = new SelectionPanel(window);
        }

        public void Update()
        {
            builderPanel.Update();
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawImage(imageSource, rect);
            playerInfoPanel.Draw(dc, window.Player, window.Units);
            builderPanel.Draw(dc);
            selectionPanel.Draw(dc, window.SelectedGameObject);
        }
    }
}