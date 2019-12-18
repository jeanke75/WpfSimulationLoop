using System;

namespace Engine
{
    class Vec3D
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float w = 1;

        public Vec3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3D(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vec3D Clone()
        {
            return new Vec3D(x, y, z);
        }

        public static Vec3D Add(Vec3D v1, Vec3D v2)
        {
            return new Vec3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vec3D Subtract(Vec3D v1, Vec3D v2)
        {
            return new Vec3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vec3D Multiply(Vec3D v, float k)
        {
            return new Vec3D(v.x * k, v.y * k, v.z * k);
        }

        public static Vec3D Divide(Vec3D v, float k)
        {
            return new Vec3D(v.x / k, v.y / k, v.z / k);
        }

        public static float DotProduct(Vec3D v1, Vec3D v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(DotProduct(this, this));
        }

        public static Vec3D Normalize(Vec3D v)
        {
            float l = v.Length();
            return new Vec3D(v.x / l, v.y / l, v.z / l);
        }

        public static Vec3D CrossProduct(Vec3D v1, Vec3D v2)
        {
            return new Vec3D(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }

        public static Vec3D Multiply(Vec3D v, Mat4x4 m)
        {
            float x = v.x * m.m[0, 0] + v.y * m.m[1, 0] + v.z * m.m[2, 0] + v.w * m.m[3, 0];
            float y = v.x * m.m[0, 1] + v.y * m.m[1, 1] + v.z * m.m[2, 1] + v.w * m.m[3, 1];
            float z = v.x * m.m[0, 2] + v.y * m.m[1, 2] + v.z * m.m[2, 2] + v.w * m.m[3, 2];
            float w = v.x * m.m[0, 3] + v.y * m.m[1, 3] + v.z * m.m[2, 3] + v.w * m.m[3, 3];
            return new Vec3D(x, y, z, w);
        }

        public static Vec3D IntersectPlane(Vec3D plane_p, Vec3D plane_n, Vec3D lineStart, Vec3D lineEnd, out float t)
        {
            plane_n = Normalize(plane_n);
            float plane_d = -DotProduct(plane_n, plane_p);
            float ad = DotProduct(lineStart, plane_n);
            float bd = DotProduct(lineEnd, plane_n);
            t = (-plane_d - ad) / (bd - ad);
            Vec3D lineStartToEnd = Subtract(lineEnd, lineStart);
            Vec3D lineToIntersect = Multiply(lineStartToEnd, t);
            return Add(lineStart, lineToIntersect);
        }
    }
}