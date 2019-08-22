using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoE
{
    internal static class Global
    {
        public static ImageSource GetImageSource(string imageId)
        {
            return BitmapDecoder.Create(new Uri("pack://application:,,,/Images/" + imageId), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
        }
    }
}