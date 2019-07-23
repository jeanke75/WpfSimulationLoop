using System;
using World.Noise;

namespace World.Generator
{
    static class WorldGenerator
    {
        private static readonly Random random = new Random();

        public static float[,] GenerateIsland(int width, int height, int octaves = 8, int subgradientCount = 0, double subgradientMinCenterOffset = 0, double subgradientMaxCenterOffset = 0.3)
        {
            float[,] gradient = GenerateGradient(width, height, subgradientCount, subgradientMinCenterOffset, subgradientMaxCenterOffset);
            float[,] noise = Perlin.Noise(width, height, octaves);
            float[,] tiles = GenerateTerrain(noise, gradient);

            return tiles;
        }

        public static float[,] GenerateWorld(int width, int height, int octaves = 9)
        {
            float[,] noise = Perlin.Noise(width, height, octaves);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noise[x, y] = noise[x, y] * 1.25f - 0.5f;
                }
            }

            return noise;
        }

        private static float[,] GenerateGradient(int width, int height, int subgradients, double minCenterOffsetSubgradient, double maxCenterOffsetSubgradient)
        {
            minCenterOffsetSubgradient = Clamp(minCenterOffsetSubgradient, 0, 1);
            maxCenterOffsetSubgradient = Clamp(maxCenterOffsetSubgradient, 0, 1);

            float[,] gradientTmp = new float[width, height];

            // Calculate the midpoint
            int centerX = width / 2 - 1;
            int centerY = height / 2 - 1;

            // Take the shortest distance from center to edge as the gradient radius
            double gradientRange = centerX <= centerY ? centerX : centerY;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double distanceX = (centerX - x) * (centerX - x);
                    double distanceY = (centerY - y) * (centerY - y);

                    double distanceToCenter = Math.Sqrt(distanceX + distanceY);

                    distanceToCenter /= gradientRange;

                    // Clamp to range 0 - 1
                    //distanceToCenter = distanceToCenter > 1 ? 1 : distanceToCenter;

                    gradientTmp[x, y] = (float)distanceToCenter;
                }
            }

            for (int i = 0; i < subgradients; i++)
            {
                // Generate a random midpoint for the extra gradient
                int extraCenterXOffset = random.Next((int)(centerX * minCenterOffsetSubgradient), (int)(centerX * minCenterOffsetSubgradient));
                int extraCenterYOffset = random.Next((int)(centerY * maxCenterOffsetSubgradient), (int)(centerY * maxCenterOffsetSubgradient));

                double extraCenterX = random.NextDouble() < 0.5 ? centerX - extraCenterXOffset : centerX + extraCenterXOffset;
                double extraCenterY = random.NextDouble() < 0.5 ? centerY - extraCenterYOffset : centerY + extraCenterYOffset;

                // Take the shortest distance from center to edge as the gradient radius
                double minDistanceX = centerX - extraCenterXOffset;
                double minDistanceY = centerY - extraCenterYOffset;
                double extraGradientRange = minDistanceX <= minDistanceY ? minDistanceX : minDistanceY;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double extraDistanceX = (extraCenterX - x) * (extraCenterX - x);
                        double extraDistanceY = (extraCenterY - y) * (extraCenterY - y);

                        double extraDistanceToCenter = Math.Sqrt(extraDistanceX + extraDistanceY);

                        extraDistanceToCenter /= extraGradientRange;

                        // Clamp to range 0 - 1
                        //extraDistanceToCenter = extraDistanceToCenter > 1 ? 1 : extraDistanceToCenter;

                        gradientTmp[x, y] = gradientTmp[x, y] <= extraDistanceToCenter ? gradientTmp[x, y] : (float)extraDistanceToCenter;
                    }
                }
            }

            return gradientTmp;
        }

        /*private static float[,] GenerateNoise(int width, int height)
        {
            float[,] noiseTmp = new float[width, height];

            float rng = random.Next(2, 5) / 1000f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseTmp[x, y] = Simplex.CalcPixel2D(x, y, rng);
                }
            }

            return noiseTmp;
        }*/

        private static float[,] GenerateTerrain(float[,] noise, float[,] gradient)
        {
            float[,] terrainTmp = new float[gradient.GetLength(0), gradient.GetLength(1)];
            for (int x = 0; x < terrainTmp.GetLength(0); x++)
            {
                for (int y = 0; y < terrainTmp.GetLength(1); y++)
                {
                    terrainTmp[x, y] = noise[x, y] - gradient[x, y];
                }
            }

            return terrainTmp;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
    }
}
