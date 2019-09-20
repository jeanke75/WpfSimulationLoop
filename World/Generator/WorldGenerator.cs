using System;
using World.Noise;

namespace World.Generator
{
    static class WorldGenerator
    {
        public static float[,] GenerateIsland(int width, int height, int octaves = 8, int subgradientCount = 0, double subgradientMinCenterOffset = 0, double subgradientMaxCenterOffset = 0.3)
        {
            float[,] gradient = Gradient.BellCurve(width, height);
            //float[,] gradient = Gradient.Linear(width, height, subgradientCount, subgradientMinCenterOffset, subgradientMaxCenterOffset);
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
    }
}
