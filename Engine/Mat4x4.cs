using System;

namespace Engine
{
    class Mat4x4
    {
        public float[,] m;

        public Mat4x4()
        {
            m = new float[4, 4];
        }

        public static Mat4x4 CreateIdentity()
        {
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = 1;
            matrix.m[1, 1] = 1;
            matrix.m[2, 2] = 1;
            matrix.m[3, 3] = 1;
            return matrix;
        }

        public static Mat4x4 CreateRotationX(float fAngleRad)
        {
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = 1;
            matrix.m[1, 1] = (float)Math.Cos(fAngleRad);
            matrix.m[1, 2] = (float)Math.Sin(fAngleRad);
            matrix.m[2, 1] = (float)-Math.Sin(fAngleRad);
            matrix.m[2, 2] = (float)Math.Cos(fAngleRad);
            matrix.m[3, 3] = 1;
            return matrix;
        }

        public static Mat4x4 CreateRotationY(float fAngleRad)
        {
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = (float)Math.Cos(fAngleRad);
            matrix.m[0, 2] = (float)Math.Sin(fAngleRad);
            matrix.m[2, 0] = (float)-Math.Sin(fAngleRad);
            matrix.m[1, 1] = 1;
            matrix.m[2, 2] = (float)Math.Cos(fAngleRad);
            matrix.m[3, 3] = 1;
            return matrix;
        }

        public static Mat4x4 CreateRotationZ(float fAngleRad)
        {
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = (float)Math.Cos(fAngleRad); ;
            matrix.m[0, 1] = (float)Math.Sin(fAngleRad);
            matrix.m[1, 0] = (float)-Math.Sin(fAngleRad);
            matrix.m[1, 1] = (float)Math.Cos(fAngleRad);
            matrix.m[2, 2] = 1;
            matrix.m[3, 3] = 1;
            return matrix;
        }

        public static Mat4x4 CreateTranslation(float x, float y, float z)
        {
            Mat4x4 matrix = CreateIdentity();
            matrix.m[3, 0] = x;
            matrix.m[3, 1] = y;
            matrix.m[3, 2] = z;
            return matrix;
        }

        public static Mat4x4 CreateProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = (float)(1.0f / Math.Tan(fFovDegrees * 0.5f / 180.0f * Math.PI));
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = fAspectRatio * fFovRad;
            matrix.m[1, 1] = fFovRad;
            matrix.m[2, 2] = fFar / (fFar - fNear);
            matrix.m[3, 2] = (-fFar * fNear) / (fFar - fNear);
            matrix.m[2, 3] = 1.0f;
            matrix.m[3, 3] = 0.0f;
            return matrix;
        }

        public static Mat4x4 Multiply(Mat4x4 m1, Mat4x4 m2)
        {
            Mat4x4 matrix = new Mat4x4();
            for (int c = 0; c < 4; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    matrix.m[r, c] = m1.m[r, 0] * m2.m[0, c] + m1.m[r, 1] * m2.m[1, c] + m1.m[r, 2] * m2.m[2, c] + m1.m[r, 3] * m2.m[3, c];
                }
            }
            return matrix;
        }

        public static Mat4x4 PointAt(Vec3D pos, Vec3D target, Vec3D up)
        {
            // Calculate new forward direction
            Vec3D newForward = Vec3D.Subtract(target, pos);
            newForward = Vec3D.Normalize(newForward);

            // Calculate new up direction
            Vec3D a = Vec3D.Multiply(newForward, Vec3D.DotProduct(up, newForward));
            Vec3D newUp = Vec3D.Subtract(up, a);
            newUp = Vec3D.Normalize(newUp);

            // Calculate new right direction
            Vec3D newRight = Vec3D.CrossProduct(newUp, newForward);

            // Construct dimensioning and translation matrix
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = newRight.x;
            matrix.m[0, 1] = newRight.y;
            matrix.m[0, 2] = newRight.z;
            matrix.m[0, 3] = 0;
            matrix.m[1, 0] = newUp.x;
            matrix.m[1, 1] = newUp.y;
            matrix.m[1, 2] = newUp.z;
            matrix.m[1, 3] = 0;
            matrix.m[2, 0] = newForward.x;
            matrix.m[2, 1] = newForward.y;
            matrix.m[2, 2] = newForward.z;
            matrix.m[2, 3] = 0;
            matrix.m[3, 0] = pos.x;
            matrix.m[3, 1] = pos.y;
            matrix.m[3, 2] = pos.z;
            matrix.m[3, 3] = 1;
            return matrix;
        }

        // Only for rotation/translation matrices
        public static Mat4x4 QuickInverse(Mat4x4 m)
        {
            Mat4x4 matrix = new Mat4x4();
            matrix.m[0, 0] = m.m[0, 0];
            matrix.m[0, 1] = m.m[1, 0];
            matrix.m[0, 2] = m.m[2, 0];
            matrix.m[0, 3] = m.m[3, 0];
            matrix.m[1, 0] = m.m[0, 1];
            matrix.m[1, 1] = m.m[1, 1];
            matrix.m[1, 2] = m.m[2, 1];
            matrix.m[1, 3] = m.m[3, 1];
            matrix.m[2, 0] = m.m[0, 2];
            matrix.m[2, 1] = m.m[1, 2];
            matrix.m[2, 2] = m.m[2, 2];
            matrix.m[2, 3] = m.m[3, 2];
            matrix.m[3, 0] = -(m.m[3, 0] * matrix.m[0, 0] + m.m[3, 1] * matrix.m[1, 0] + m.m[3, 2] * matrix.m[2, 0] + m.m[3, 3] * matrix.m[3, 0]);
            matrix.m[3, 1] = -(m.m[3, 0] * matrix.m[0, 1] + m.m[3, 1] * matrix.m[1, 1] + m.m[3, 2] * matrix.m[2, 1] + m.m[3, 3] * matrix.m[3, 1]);
            matrix.m[3, 2] = -(m.m[3, 0] * matrix.m[0, 2] + m.m[3, 1] * matrix.m[1, 2] + m.m[3, 2] * matrix.m[2, 2] + m.m[3, 3] * matrix.m[3, 2]);
            matrix.m[3, 3] = 1;
            return matrix;
        }
    }
}