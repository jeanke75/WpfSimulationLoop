using System;
using System.Collections.Generic;
using System.IO;

namespace Engine
{
    class Mesh
    {
        public List<Triangle> tris;

        public Mesh(List<Triangle> tris)
        {
            this.tris = tris ?? throw new Exception("Tris can't be null");
        }

        public static Mesh Cube()
        {
            Vec3D v1 = new Vec3D(0, 0, 0, 1);
            Vec3D v2 = new Vec3D(0, 1, 0, 1);
            Vec3D v3 = new Vec3D(1, 1, 0, 1);
            Vec3D v4 = new Vec3D(1, 0, 0, 1);
            Vec3D v5 = new Vec3D(0, 0, 1, 1);
            Vec3D v6 = new Vec3D(0, 1, 1, 1);
            Vec3D v7 = new Vec3D(1, 1, 1, 1);
            Vec3D v8 = new Vec3D(1, 0, 1, 1);

            Mesh meshCube = new Mesh(new List<Triangle>{
                // SOUTH
                new Triangle(v1, v2, v3, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v1, v3, v4, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1)),

                // EAST
                new Triangle(v4, v3, v7, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v4, v7, v8, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1)),

                // NORTH
                new Triangle(v8, v7, v6, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v8, v6, v5, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1)),

                // WEST
                new Triangle(v5, v6, v2, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v5, v2, v1, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1)),

                // TOP
                new Triangle(v2, v6, v7, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v2, v7, v3, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1)),

                // BOTTOM
                new Triangle(v8, v5, v1, new Vec2D(0, 1), new Vec2D(0, 0), new Vec2D(1, 0)),
                new Triangle(v8, v1, v4, new Vec2D(0, 1), new Vec2D(1, 0), new Vec2D(1, 1))
            });

            return meshCube;
        }

        public static Mesh LoadFromObjectFile(string path)
        {
            List<Triangle> tris = new List<Triangle>();
            using (StreamReader file = new StreamReader(path))
            {
                string ln;

                List<Vec3D> verts = new List<Vec3D>();

                while ((ln = file.ReadLine()) != null)
                {
                    string[] line = ln.Split();
                    if (line[0] == "v")
                    {
                        verts.Add(new Vec3D(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])));
                    }
                    else if (line[0] == "f")
                    {
                        tris.Add(new Triangle(verts[int.Parse(line[1]) - 1], verts[int.Parse(line[2]) - 1], verts[int.Parse(line[3]) - 1]));
                    }
                }
                file.Close();
            }

            return new Mesh(tris);
        }
    }
}