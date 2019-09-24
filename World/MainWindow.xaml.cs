using DrawingBase;
using DrawingBase.Input;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
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
        private Vector3 cameraPos;
        private bool maximizeMinimap = false;
        private ImageSource mapSource;
        private ImageSource frontViewSource;
        

        public override void Initialize()
        {
            int size = 1024;
            SetResolution(size, size);

            cameraPos = new Vector3(size / 2f, 0.5f, size);

            float[,] heightmap = WorldGenerator.GenerateIsland(size, size);

            mapSource = GenerateImage(heightmap);
            //frontViewSource = GenerateFrontView(heightmap);
            frontViewSource = GenerateFrontViewMesh(heightmap);
        }

        private ImageSource GenerateFrontViewMesh(float[,] heightmap)
        {
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Vector3 camera = new Vector3(cameraPos.X * 5f, cameraPos.Y * 1000f, cameraPos.Z * 5f);
                for (int x = 0; x < heightmap.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < heightmap.GetLength(1) - 1; y++)
                    {
                        // Get points
                        Vector3 vTopLeft = new Vector3(x * 5f, GetHeight() - heightmap[x, y] * 1000f, y * 5f);
                        Vector3 vTopRight = new Vector3((x + 1) * 5f, GetHeight() - heightmap[x + 1, y] * 1000f, y * 5f);
                        Vector3 vBottomLeft = new Vector3(x * 5f, GetHeight() - heightmap[x, y + 1] * 1000f, y * 5f);
                        Vector3 vBottomRight = new Vector3((x + 1) * 5f, GetHeight() - heightmap[x + 1, y + 1] * 1000f, y * 5f);

                        bool drawLeftTriangle = IsTriangleVisible(vTopLeft, vTopRight, vBottomLeft, camera);
                        if (drawLeftTriangle)
                        {
                            dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(vTopLeft.X, vTopLeft.Y), new Point(vTopRight.X, vTopRight.Y));
                            dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(vTopRight.X, vTopRight.Y), new Point(vBottomLeft.X, vBottomLeft.Y));
                            dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(vBottomLeft.X, vBottomLeft.Y), new Point(vTopLeft.X, vTopLeft.Y));
                        }

                        bool drawRightTriangle = IsTriangleVisible(vTopRight, vBottomRight, vBottomLeft, camera);
                        if (drawRightTriangle)
                        {
                            dc.DrawLine(new Pen(Brushes.LightGray, 1), new Point(vTopRight.X, vTopRight.Y), new Point(vBottomRight.X, vBottomRight.Y));
                            dc.DrawLine(new Pen(Brushes.LightGray, 1), new Point(vBottomRight.X, vBottomRight.Y), new Point(vBottomLeft.X, vBottomLeft.Y));
                            dc.DrawLine(new Pen(Brushes.LightGray, 1), new Point(vBottomLeft.X, vBottomLeft.Y), new Point(vTopRight.X, vTopRight.Y));
                        }
                    }
                }
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(GetWidth() * 5, GetHeight(), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(dv);
            Image img = new Image
            {
                Source = renderTargetBitmap
            };

            return img.Source;
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (InputHelper.Keyboard.GetPressedState(System.Windows.Input.Key.F3) == ButtonState.Pressed)
            {
                maximizeMinimap = !maximizeMinimap;
            }
        }

        public override void Draw(DrawingContext dc)
        {
            if (!maximizeMinimap)
            {
                dc.DrawImage(frontViewSource, new Rect(-1000, 0, GetWidth() * 5, GetHeight()));
                Rect r = new Rect(GetWidth() * 0.85f, 0, GetWidth() * 0.15f, GetHeight() * 0.15f);
                dc.DrawRectangle(Brushes.Black, null, r);
                dc.DrawImage(mapSource, r);
            }
            else
                dc.DrawImage(mapSource, new Rect(0, 0, GetWidth(), GetHeight()));
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
                        float requiredHeightToDrawPixel = (GetHeight() - y) / (float)GetHeight() * 2.5f;
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

        private bool IsTriangleVisible(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 camera)
        {
            // Get plane normal
            Vector3 vectorAB = point2 - point1;
            Vector3 vectorAC = point3 - point1;
            float nx = (vectorAB.Y * vectorAC.Z - vectorAB.Z * vectorAC.Y);
            float ny = (vectorAB.Z * vectorAC.X - vectorAB.X * vectorAC.Z);
            float nz = (vectorAB.X * vectorAC.Y - vectorAB.Y * vectorAC.X);

            float d = -(nx * point1.X + ny * point1.Y + nz * point1.Z);

            // Check if plane is facing towards the camera
            float res = nx * camera.X + ny * camera.Y + nz * camera.Z + d;

            return res < 0;
        }
    }
}