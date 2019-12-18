using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Engine
{
    class Triangle
    {
        public Vec3D[] p;
        public Vec2D[] t;
        public Color col;

        public Triangle(Vec3D v1, Vec3D v2, Vec3D v3)
        {
            p = new Vec3D[] { v1, v2, v3 };
            t = new Vec2D[3];
        }

        public Triangle(Vec3D v1, Vec3D v2, Vec3D v3, Vec2D t1, Vec2D t2, Vec2D t3)
        {
            p = new Vec3D[] { v1, v2, v3 };
            t = new Vec2D[] { t1, t2, t3 };
        }

        public Vec3D GetNormal()
        {
            Vec3D line1 = Vec3D.Subtract(p[1], p[0]);
            Vec3D line2 = Vec3D.Subtract(p[2], p[0]);
            return Vec3D.CrossProduct(line1, line2);
        }

        public Triangle Clone()
        {
            return new Triangle(p[0].Clone(), p[1].Clone(), p[2].Clone());
        }

        public static List<Triangle> ClipAgainstPlane(Vec3D plane_p, Vec3D plane_n, Triangle in_tri)
        {
            List<Triangle> clipped = new List<Triangle>();

            // Make sure plane normal is indeed normal
            plane_n = Vec3D.Normalize(plane_n);

            // Return signed shortest distance from point to plane, plane normal must be normal
            float Dist(Vec3D p)
            {
                Vec3D n = Vec3D.Normalize(p);
                return plane_n.x * p.x + plane_n.y * p.y + plane_n.z * p.z - Vec3D.DotProduct(plane_n, plane_p);
            }

            // Create two temporary storage arrays to classify points either side of plane
            // If distance sign is positive, point lies on "inside" of plane
            Vec3D[] inside_points = new Vec3D[3];
            int nInsidePointCount = 0;
            Vec3D[] outside_points = new Vec3D[3];
            int nOutsidePointCount = 0;
            Vec2D[] inside_textures = new Vec2D[3];
            int nInsideTexCount = 0;
            Vec2D[] outside_textures = new Vec2D[3];
            int nOutsideTexCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = Dist(in_tri.p[0]);
            float d1 = Dist(in_tri.p[1]);
            float d2 = Dist(in_tri.p[2]);

            if (d0 >= 0)
            {
                inside_points[nInsidePointCount++] = in_tri.p[0];
                inside_textures[nInsideTexCount++] = in_tri.t[0];
            }
            else
            {
                outside_points[nOutsidePointCount++] = in_tri.p[0];
                outside_textures[nOutsideTexCount++] = in_tri.t[0];
            }
            if (d1 >= 0)
            {
                inside_points[nInsidePointCount++] = in_tri.p[1];
                inside_textures[nInsideTexCount++] = in_tri.t[1];
            }
            else
            {
                outside_points[nOutsidePointCount++] = in_tri.p[1];
                outside_textures[nOutsideTexCount++] = in_tri.t[1];
            }
            if (d2 >= 0)
            {
                inside_points[nInsidePointCount++] = in_tri.p[2];
                inside_textures[nInsideTexCount++] = in_tri.t[2];
            }
            else
            {
                outside_points[nOutsidePointCount++] = in_tri.p[2];
                outside_textures[nOutsideTexCount++] = in_tri.t[2];
            }

            // Now classify triangle points, and break the input triangle into
            // smaller output triangles if required. There are four possible
            // outcomes...

            if (nInsidePointCount == 0)
            {
                // All points lie on the outside of the plane, so clip whole triangle
                // It ceases to exist
                return clipped;
            }
            else if (nInsidePointCount == 3)
            {
                // All points lie on the intside of the plane, so do nothing
                // and allow the triangle to simply pass through
                clipped.Add(in_tri);
            }
            else if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                float t;
                // Triangle should be clipped. As two points lie outside
                // the plane, the triangle simple becomes a smaller triangle

                // The inside point is valid, so keep that...
                Vec3D inside_point = inside_points[0];
                Vec2D inside_tex = inside_textures[0];

                // but the two new points are at the locations where the
                // original sides of the triangle (lines) intersect with the plane
                Vec3D p2 = Vec3D.IntersectPlane(plane_p, plane_n, inside_point, outside_points[0], out t);
                Vec2D t2 = new Vec2D(t * (outside_textures[0].u - inside_textures[0].u) + inside_textures[0].u, t * (outside_textures[0].v - inside_textures[0].v) + inside_textures[0].v);

                Vec3D p3 = Vec3D.IntersectPlane(plane_p, plane_n, inside_point, outside_points[1], out t);
                Vec2D t3 = new Vec2D(t * (outside_textures[1].u - inside_textures[0].u) + inside_textures[0].u, t * (outside_textures[1].v - inside_textures[0].v) + inside_textures[0].v);

                Triangle clippedTri = new Triangle(inside_point, p2, p3, inside_tex, t2, t3)
                {
                    // Copy appearance info to new triangle
                    col = in_tri.col
                };

                clipped.Add(clippedTri);
            }
            else if (nInsidePointCount == 2 && nOutsidePointCount == 1)
            {
                float t;
                // Triangle should be clipped. As two points lie inside the plane,
                // the clipped triangle becomes a "quad". Fortunately, we can
                // represent a quad with two new triangles

                // The first triangle consists of the two inside points and a new
                // point determined by the location where one side of the triangle
                // intersects with the plane
                Vec3D v1p3 = Vec3D.IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0], out t);
                Vec2D v1t3 = new Vec2D(t * (outside_textures[0].u - inside_textures[0].u) + inside_textures[0].u, t * (outside_textures[0].v - inside_textures[0].v) + inside_textures[0].v);
                Triangle clippedTri1 = new Triangle(inside_points[0], inside_points[1], v1p3, inside_textures[0], inside_textures[1], v1t3)
                {
                    // Copy appearance info to new triangles
                    col = in_tri.col
                };

                clipped.Add(clippedTri1);

                // The second triangle is composed of one of the inside points, a
                // new point determined by the intersection of the other side of the 
                // triangle and the plane, and the newly created point above
                Vec3D v2p3 = Vec3D.IntersectPlane(plane_p, plane_n, inside_points[1], outside_points[0], out t);
                Vec2D v2t3 = new Vec2D(t * (outside_textures[0].u - inside_textures[1].u) + inside_textures[1].u, t * (outside_textures[0].v - inside_textures[1].v) + inside_textures[1].v);
                Triangle clippedTri2 = new Triangle(inside_points[1], clippedTri1.p[2], v2p3, inside_textures[1], outside_textures[2], v2t3)
                {
                    // Copy appearance info to new triangles
                    col = in_tri.col
                };
                
                clipped.Add(clippedTri2);
            }
            else
                throw new Exception("Shouldn't happen");

            return clipped;
        }
    }
}