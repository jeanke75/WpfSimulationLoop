using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        Mesh mesh;
        Mat4x4 matProj;

        Vec3D vCamera;
        Vec3D vLookDir;

        float fYaw;

        float fTheta;

        BitmapSource sprText1;
        WriteableBitmap img;

        readonly bool showWireframe = false;

        public override void Initialize()
        {
            int size = 1024;
            SetResolution(size, size);
            DisplayInfo = true;

            mesh = Mesh.Cube();
            //mesh = Mesh.LoadFromObjectFile(@"C:\Users\tomjan\Desktop\Test.obj");

            sprText1 = GetImageSource(@"C:\Users\tomjan\Desktop\Test.jpg");

            // Projection matrix
            matProj = Mat4x4.CreateProjection(90.0f, (float)GetHeight() / (float)GetWidth(), 0.1f, 10.0f);

            vCamera = new Vec3D(0, 0, 0);
            vLookDir = new Vec3D(0, 0, 1);
        }

        public override void Update(float dt)
        {
            fTheta += 1.0f * dt;
            InputHelper.Update();

            if (InputHelper.Keyboard.GetPressedState(Key.Up) == ButtonState.Down)
                vCamera.y -= 8f * dt;
            else if (InputHelper.Keyboard.GetPressedState(Key.Down) == ButtonState.Down)
                vCamera.y += 8f * dt;
            if (InputHelper.Keyboard.GetPressedState(Key.Left) == ButtonState.Down)
                vCamera.x -= 8f * dt;
            else if (InputHelper.Keyboard.GetPressedState(Key.Right) == ButtonState.Down)
                vCamera.x += 8f * dt;

            Vec3D vForward = Vec3D.Multiply(vLookDir, 8f * dt);

            if (InputHelper.Keyboard.GetPressedState(Key.Z) == ButtonState.Down)
                vCamera = Vec3D.Add(vCamera, vForward);
            else if (InputHelper.Keyboard.GetPressedState(Key.S) == ButtonState.Down)
                vCamera = Vec3D.Subtract(vCamera, vForward);
            if (InputHelper.Keyboard.GetPressedState(Key.Q) == ButtonState.Down)
                fYaw += 2.0f * dt;
            else if (InputHelper.Keyboard.GetPressedState(Key.D) == ButtonState.Down)
                fYaw -= 2.0f * dt;
        }

        public override void Draw(DrawingContext dc)
        {
            img = new WriteableBitmap(GetWidth(), GetHeight(), 96, 96, PixelFormats.Pbgra32, null);

            // Set up rotation matrices
            Mat4x4 matRotZ = Mat4x4.CreateRotationZ(fTheta * 0.5f);
            Mat4x4 matRotX = Mat4x4.CreateRotationX(fTheta);

            Mat4x4 matTrans = Mat4x4.CreateTranslation(0, 0, 5.0f);

            Mat4x4 matWorld = Mat4x4.CreateIdentity();
            //matWorld = Mat4x4.Multiply(matRotZ, matRotX);
            matWorld = Mat4x4.Multiply(matWorld, matTrans);

            Vec3D vUp = new Vec3D(0, 1, 0);
            Vec3D vTarget = new Vec3D(0, 0, 1);
            Mat4x4 matCameraRot = Mat4x4.CreateRotationY(fYaw);
            vLookDir = Vec3D.Multiply(vTarget, matCameraRot);
            vTarget = Vec3D.Add(vCamera, vLookDir);

            Mat4x4 matCamera = Mat4x4.PointAt(vCamera, vTarget, vUp);

            // Make view matrix from camera
            Mat4x4 matView = Mat4x4.QuickInverse(matCamera);

            // Store triangles for rastering later
            List<Triangle> vecTrianglesToRaster = new List<Triangle>(); ;

            // Draw Triangles
            foreach (Triangle tri in mesh.tris)
            {
                Vec3D v1Transformed = Vec3D.Multiply(tri.p[0], matWorld);
                Vec3D v2Transformed = Vec3D.Multiply(tri.p[1], matWorld);
                Vec3D v3Transformed = Vec3D.Multiply(tri.p[2], matWorld);
                Triangle triTransformed = new Triangle(v1Transformed, v2Transformed, v3Transformed, tri.t[0], tri.t[1], tri.t[2]);

                // Get the triangle normal and normalize it
                Vec3D normal = triTransformed.GetNormal();
                normal = Vec3D.Normalize(normal);

                // Get ray from triangle to camera
                Vec3D vCameraRay = Vec3D.Subtract(triTransformed.p[0], vCamera);

                // If ray is aligned with normal, then triangle is visible
                if (Vec3D.DotProduct(normal, vCameraRay) < 0)
                {
                    // Illumination
                    Vec3D light_direction = new Vec3D(0, 0, -1);
                    light_direction = Vec3D.Normalize(light_direction);

                    // How "aligned" are the light direction and triangle surface normal?
                    float dp = Math.Max(0.1f, Vec3D.DotProduct(light_direction, normal));

                    // Choose required color
                    triTransformed.col = SetColorBrightness(Colors.White, dp);

                    // Convert World Space -> View Space
                    Vec3D v1Viewed = Vec3D.Multiply(triTransformed.p[0], matView);
                    Vec3D v2Viewed = Vec3D.Multiply(triTransformed.p[1], matView);
                    Vec3D v3Viewed = Vec3D.Multiply(triTransformed.p[2], matView);
                    Triangle triViewed = new Triangle(v1Viewed, v2Viewed, v3Viewed, triTransformed.t[0], triTransformed.t[1], triTransformed.t[2])
                    {
                        col = triTransformed.col
                    };

                    // Clip viewed triangle against near plane, this could form two
                    // additional triangles
                    List<Triangle> clippedTriangles = Triangle.ClipAgainstPlane(new Vec3D(0, 0, 0.1f), new Vec3D(0, 0, 1), triViewed);

                    foreach (Triangle clippedTri in clippedTriangles)
                    {
                        // Project triangles from 3D -> 2D
                        Vec3D v1Projected = Vec3D.Multiply(clippedTri.p[0], matProj);
                        Vec3D v2Projected = Vec3D.Multiply(clippedTri.p[1], matProj);
                        Vec3D v3Projected = Vec3D.Multiply(clippedTri.p[2], matProj);
                        Triangle triProjected = new Triangle(v1Projected, v2Projected, v3Projected, clippedTri.t[0], clippedTri.t[1], clippedTri.t[2])
                        {
                            col = clippedTri.col
                        };

                        // Scale into view
                        triProjected.p[0] = Vec3D.Divide(triProjected.p[0], triProjected.p[0].w);
                        triProjected.p[1] = Vec3D.Divide(triProjected.p[1], triProjected.p[1].w);
                        triProjected.p[2] = Vec3D.Divide(triProjected.p[2], triProjected.p[2].w);

                        // Offset verts into visible normalised space
                        Vec3D vOffsetView = new Vec3D(1, 1, 0);
                        triProjected.p[0] = Vec3D.Add(triProjected.p[0], vOffsetView);
                        triProjected.p[1] = Vec3D.Add(triProjected.p[1], vOffsetView);
                        triProjected.p[2] = Vec3D.Add(triProjected.p[2], vOffsetView);
                        triProjected.p[0].x *= 0.5f * GetWidth();
                        triProjected.p[0].y *= 0.5f * GetHeight();
                        triProjected.p[1].x *= 0.5f * GetWidth();
                        triProjected.p[1].y *= 0.5f * GetHeight();
                        triProjected.p[2].x *= 0.5f * GetWidth();
                        triProjected.p[2].y *= 0.5f * GetHeight();

                        // Store triangle for sorting
                        vecTrianglesToRaster.Add(triProjected);
                    }
                }
            }

            // Sort triangles from back to front;
            vecTrianglesToRaster.Sort((t1, t2) => {
                float z1 = (t1.p[0].z + t1.p[1].z + t1.p[2].z) / 3.0f;
                float z2 = (t2.p[0].z + t2.p[1].z + t2.p[2].z) / 3.0f;
                return z2.CompareTo(z1);
            });

            foreach (Triangle triToRaster in vecTrianglesToRaster)
            {
                // Clip triangles against all four screen edges, this could yield
                // a bunch of triangles
                List<Triangle> clipped = null;
                Queue<Triangle> listTriangles = new Queue<Triangle>();
                listTriangles.Enqueue(triToRaster);
                int nNewTriangles = 1;

                for (int p = 0; p < 4; p++)
                {
                    int nTrisToAdd = 0;
                    while (nNewTriangles > 0)
                    {
                        Triangle test = listTriangles.Dequeue();
                        nNewTriangles--;

                        switch (p)
                        {
                            case 0:
                                clipped = Triangle.ClipAgainstPlane(new Vec3D(0, 0, 0), new Vec3D(0, 1, 0), test);
                                nTrisToAdd = clipped.Count;
                                break;
                            case 1:
                                clipped = Triangle.ClipAgainstPlane(new Vec3D(0, GetHeight() - 1, 0), new Vec3D(0, -1, 0), test);
                                nTrisToAdd = clipped.Count;
                                break;
                            case 2:
                                clipped = Triangle.ClipAgainstPlane(new Vec3D(0, 0, 0), new Vec3D(1, 0, 0), test);
                                nTrisToAdd = clipped.Count;
                                break;
                            case 3:
                                clipped = Triangle.ClipAgainstPlane(new Vec3D(GetWidth() - 1, 0, 0), new Vec3D(-1, 0, 0), test);
                                nTrisToAdd = clipped.Count;
                                break;
                        }

                        foreach (Triangle t in clipped)
                            listTriangles.Enqueue(t);
                    }
                    nNewTriangles = listTriangles.Count;
                }


                // Draw the transformed, viewed, clipped, projected, sorted, clipped triangles
                
                foreach (Triangle t in listTriangles)
                {
                    Brush brush = new SolidColorBrush(triToRaster.col);
                    Brush wireFrame = showWireframe ? Brushes.Black : brush;
                    //dc.DrawTriangle(brush, null/*new Pen(wireFrame, 1)*/, new Point(triToRaster.p[0].x, triToRaster.p[0].y), new Point(triToRaster.p[1].x, triToRaster.p[1].y), new Point(triToRaster.p[2].x, triToRaster.p[2].y));
                    DrawTexturedTriangle((int)triToRaster.p[0].x, (int)triToRaster.p[0].y, triToRaster.t[0].u, triToRaster.t[0].v,
                                         (int)triToRaster.p[1].x, (int)triToRaster.p[1].y, triToRaster.t[1].u, triToRaster.t[1].v,
                                         (int)triToRaster.p[2].x, (int)triToRaster.p[2].y, triToRaster.t[2].u, triToRaster.t[2].v,
                                         sprText1);
                    dc.DrawTriangle(null, new Pen(Brushes.White, 1), new Point(triToRaster.p[0].x, triToRaster.p[0].y), new Point(triToRaster.p[1].x, triToRaster.p[1].y), new Point(triToRaster.p[2].x, triToRaster.p[2].y));
                } 
            }
        }

        public override void Cleanup()
        {

        }

        private Color SetColorBrightness(Color color, float brightness)
        {
            if (brightness < 0)
                brightness = 0;
            else if (brightness > 1)
                brightness = 1;

            color.R = (byte)(color.R * brightness);
            color.G = (byte)(color.G * brightness);
            color.B = (byte)(color.B * brightness);

            return color;
        }

        private void DrawTexturedTriangle(int x1, int y1, float u1, float v1,
                                          int x2, int y2, float u2, float v2,
                                          int x3, int y3, float u3, float v3,
                                          BitmapSource tex)
        {
            if (y2 < y1)
            {
                Swap(ref y1, ref y2);
                Swap(ref x1, ref x2);
                Swap(ref u1, ref u2);
                Swap(ref v1, ref v2);
            }

            if (y3 < y1)
            {
                Swap(ref y1, ref y3);
                Swap(ref x1, ref x3);
                Swap(ref u1, ref u3);
                Swap(ref v1, ref v3);
            }

            if (y3 < y2)
            {
                Swap(ref y2, ref y3);
                Swap(ref x2, ref x3);
                Swap(ref u2, ref u3);
                Swap(ref v2, ref v3);
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;
            float dv1 = v2 - v1;
            float du1 = u2 - u1;

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;
            float dv2 = v3 - v1;
            float du2 = u3 - u1;

            float tex_u, tex_v;

            float dax_step = 0;
            float dbx_step = 0;
            float du1_step = 0;
            float dv1_step = 0;
            float du2_step = 0;
            float dv2_step = 0;

            if (dy1 > 0)
                dax_step = dx1 / (float)Math.Abs(dy1);
            if (dy2 > 0)
                dbx_step = dx2 / (float)Math.Abs(dy2);
            if (dy1 > 0)
                du1_step = du1 / (float)Math.Abs(dy1);
            if (dy1 > 0)
                dv1_step = dv1 / (float)Math.Abs(dy1);
            if (dy2 > 0)
                du2_step = du2 / (float)Math.Abs(dy2);
            if (dy2 > 0)
                dv2_step = dv2 / (float)Math.Abs(dy2);
            if (dy1 > 0)
            {
                for (int i = y1; i <= y2; i++)
                {
                    int ax = (int)(x1 + (i - y1) * dax_step);
                    int bx = (int)(x1 + (i - y1) * dbx_step);

                    float tex_su = u1 + (i - y1) * du1_step;
                    float tex_sv = v1 + (i - y1) * dv1_step;

                    float tex_eu = u1 + (i - y1) * du2_step;
                    float tex_ev = v1 + (i - y1) * dv2_step;

                    if (ax > bx)
                    {
                        Swap(ref ax, ref bx);
                        Swap(ref tex_su, ref tex_eu);
                        Swap(ref tex_sv, ref tex_ev);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;

                    float tstep = 1 / (float)(bx - ax);
                    float t = 0;

                    for (int j = ax; j < bx; j++)
                    {
                        tex_u = (1 - t) * tex_su + t * tex_eu;
                        tex_v = (1 - t) * tex_sv + t * tex_ev;

                        DrawPixel(j, i, GetPixelColor(tex, (int)tex_u, (int)tex_v));

                        t += tstep;
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            dv1 = v3 - v2;
            du1 = u3 - u2;

            if (dy1 > 0)
                dax_step = dx1 / (float)Math.Abs(dy1);
            if (dy2 > 0)
                dax_step = dx2 / (float)Math.Abs(dy2);

            du1_step = 0;
            dv1_step = 0;

            if (dy1 > 0)
                du1_step = du1 / (float)Math.Abs(dy1);
            if (dy1 > 0)
                dv1_step = dv1 / (float)Math.Abs(dy1);

            if (dy1 > 0)
            {
                for (int i = y2; i <= y3; i++)
                {
                    int ax = (int)(x2 + (i - y2) * dax_step);
                    int bx = (int)(x1 + (i - y1) * dbx_step);

                    float tex_su = u2 + (i - y2) * du1_step;
                    float tex_sv = v2 + (i - y2) * dv1_step;

                    float tex_eu = u1 + (i - y1) * du2_step;
                    float tex_ev = v1 + (i - y1) * dv2_step;

                    if (ax > bx)
                    {
                        Swap(ref ax, ref bx);
                        Swap(ref tex_su, ref tex_eu);
                        Swap(ref tex_sv, ref tex_ev);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;

                    float tstep = 1 / (float)(bx - ax);
                    float t = 0;

                    for (int j = ax; j < bx; j++)
                    {
                        tex_u = (1 - t) * tex_su + t * tex_eu;
                        tex_v = (1 - t) * tex_sv + t * tex_ev;

                        DrawPixel(j, i, GetPixelColor(tex, (int)tex_u, (int)tex_v));

                        t += tstep;
                    }
                }
            }
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private BitmapSource GetImageSource(string imageId)
        {
            return BitmapDecoder.Create(new Uri(imageId), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
        }

        /*private BitmapSource GetImageSourceFromApplicationFolder(string imageId)
        {
            return GetImageSource("pack://application:,,,/Images/" + imageId);
        }*/

        private Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            Color color;
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel];
            var rect = new Int32Rect(x, y, 1, 1);

            bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

            if (bitmap.Format == PixelFormats.Bgra32)
            {
                color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else if (bitmap.Format == PixelFormats.Bgr32)
            {
                color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
            }
            // handle other required formats
            else
            {
                color = Colors.Black;
            }

            return color;
        }

        private void DrawPixel(int x, int y, Color color)
        {
            try
            {
                // Reserve the back buffer for updates.
                img.Lock();

                // Update the pixel value
                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = img.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += y * img.BackBufferStride;
                    pBackBuffer += x * 4;

                    // Compute the pixel's color.
                    int color_data = color.R << 16; // R
                    color_data |= color.G << 8;   // G
                    color_data |= color.B << 0;   // B

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                img.Unlock();
            }
        }
    }
}