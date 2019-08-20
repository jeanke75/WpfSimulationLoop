using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoE.UI
{
    class UserInterface
    {
        private readonly MainWindow window;

        private readonly ImageSource imageSource;
        private readonly Rect rect;

        private readonly PlayerInfoPanel playerInfoPanel;
        private readonly SelectionPanel selectionPanel;

        public UserInterface(MainWindow window)
        {
            this.window = window;

            imageSource = GetImageSource("UserInterface.png");


            rect = new Rect(0, 0, window.GetWidth(), window.GetHeight());
            playerInfoPanel = new PlayerInfoPanel(window);
            selectionPanel = new SelectionPanel(window);
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawImage(imageSource, rect);
            playerInfoPanel.Draw(dc, window.player, window.units);
            selectionPanel.Draw(dc, window.selectedGameObject);
        }

        private ImageSource GetImageSource(string imageId)
        {
            return BitmapDecoder.Create(new Uri("pack://application:,,,/Images/UI/" + imageId), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
        }
    }
}