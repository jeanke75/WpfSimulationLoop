using DrawingBase;
using DrawingBase.Input;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using World.Generator;

namespace World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private bool showFront = false;
        private ImageSource terrainSource;
        private ImageSource frontViewSource;

        public override void Initialize()
        {
            int size = 1024;
            SetResolution(size, size);

            float[,] heightmap = WorldGenerator.GenerateIsland(size, size);

            terrainSource = GenerateImage(heightmap);
            frontViewSource = GenerateFrontView(heightmap);
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (InputHelper.Keyboard.GetPressedState(System.Windows.Input.Key.F3) == ButtonState.Pressed)
            {
                showFront = !showFront;
            }
        }

        public override void Draw(DrawingContext dc)
        {
            if (showFront)
                dc.DrawImage(frontViewSource, new Rect(0, 0, GetWidth(), GetHeight()));
            else
                dc.DrawImage(terrainSource, new Rect(0, 0, GetWidth(), GetHeight()));
        }

        public override void Cleanup()
        {
        }

        private ImageSource GenerateImage(float[,] data)
        {
            WriteableBitmap bmp = new WriteableBitmap(GetWidth(), GetHeight(), 96, 96, PixelFormats.Pbgra32, null);
            try
            {
                // Reserve the back buffer for updates.
                bmp.Lock();

                // Update the pixel values
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    for (int y = 0; y < data.GetLength(1); y++)
                    {
                        Color fill;// = ChangeColorBrightness(Colors.Gray, data[x, y] * 2 - 1);
                        if (data[x, y] <= -0.25)
                            fill = ChangeColorBrightness(Colors.DarkBlue, (data[x, y] + 0.4f) / 2f);
                        else if (data[x, y] < 0)
                            fill = ChangeColorBrightness(Colors.Blue, data[x, y] * 1.5f);
                        else if (data[x, y] < 0.03)
                            fill = Colors.SandyBrown;
                        else if (data[x, y] < 0.10)
                            fill = Colors.SaddleBrown;
                        else if (data[x, y] < 0.20)
                            fill = Colors.ForestGreen;
                        else if (data[x, y] < 0.30)
                            fill = Colors.Green;
                        else if (data[x, y] < 0.45)
                            fill = ChangeColorBrightness(Colors.Gray, 0.7f - data[x, y]);
                        else
                            fill = Colors.Snow;

                        SetPixelColor(bmp, x, y, fill);
                    }
                }

                // Specify the area of the bitmap that changed. (not used here, but tells the ui what pixels to update)
                bmp.AddDirtyRect(new Int32Rect(0, 0, data.GetLength(0), data.GetLength(1)));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bmp.Unlock();
            }

            return bmp;
        }

        private ImageSource GenerateFrontView(float[,] data)
        {
            WriteableBitmap bmp = new WriteableBitmap(GetWidth(), GetHeight(), 96, 96, PixelFormats.Pbgra32, null);
            try
            {
                // Reserve the back buffer for updates.
                bmp.Lock();

                // shoot a "ray" through each pixel of the final image and return the distance to the first part of land it hits
                // color darker the further it is away

                for (int x = 0; x < data.GetLength(0); x++)
                {
                    for (int y = data.GetLength(1) - 1; y >= 0; y--)
                    {
                        float requiredHeightToDrawPixel = (GetHeight() - y) / (float)GetHeight();
                        int distance = -1;
                        for (int cy = data.GetLength(1) - 1; cy >= 0; cy--)
                        {
                            if (data[x, cy] >= requiredHeightToDrawPixel)
                            {
                                distance = data.GetLength(1) - cy;
                                break;
                            }
                        }

                        Color fill = Colors.SkyBlue;
                        if (distance > -1)
                        {
                            /*if (requiredHeightToDrawPixel <= -0.25)
                                fill = ChangeColorBrightness(Colors.DarkBlue, (data[x, y] + 0.4f) / 2f);
                            else if (requiredHeightToDrawPixel < 0)
                                fill = ChangeColorBrightness(Colors.Blue, data[x, y] * 1.5f);
                            else if (requiredHeightToDrawPixel < 0.03)
                                fill = Colors.SandyBrown;
                            else if (requiredHeightToDrawPixel < 0.10)
                                fill = Colors.SaddleBrown;
                            else if (requiredHeightToDrawPixel < 0.20)
                                fill = Colors.ForestGreen;
                            else if (requiredHeightToDrawPixel < 0.30)
                                fill = Colors.Green;
                            else if (requiredHeightToDrawPixel < 0.45)
                                fill = ChangeColorBrightness(Colors.Gray, 0.7f - data[x, y]);
                            else
                                fill = Colors.Snow;*/
                            fill = Colors.Gray;
                        }
                        SetPixelColor(bmp, x, y, ChangeColorBrightness(fill, -(distance / (float)data.GetLength(1))));
                    }
                }

                // Specify the area of the bitmap that changed. (not used here, but tells the ui what pixels to update)
                bmp.AddDirtyRect(new Int32Rect(0, 0, data.GetLength(0), data.GetLength(1)));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bmp.Unlock();
            }

            return bmp;
        }

        private void SetPixelColor(WriteableBitmap bmp, int x, int y, Color fill)
        {
            unsafe
            {
                // Get a pointer to the back buffer.
                IntPtr pBackBuffer = bmp.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * bmp.BackBufferStride;
                pBackBuffer += x * 4;

                // Compute the pixel's color.
                int color_data = fill.R << 16; // R
                color_data |= fill.G << 8;   // G
                color_data |= fill.B << 0;   // B

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = color_data;
            }
        }

        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}