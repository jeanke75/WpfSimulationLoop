using System;

namespace World
{
    static class Gradient
    {
        private static readonly Random random = new Random();

        public static float[,] Linear(int width, int height, int subgradients, double minCenterOffsetSubgradient, double maxCenterOffsetSubgradient)
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

        public static float[,] BellCurve(int width, int height, float peakHeight)
        {
            float[,] result = new float[width, height];
            double midPointX = width / 2d;
            double midPointY = height / 2d;
            double midPoint = Math.Sqrt(midPointX * midPointX + midPointY * midPointY);
            double standardDeviation = midPoint / 4f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        result[x, y] = 0;
                        continue;
                    }
                    double distToCenterOfCurve = Math.Sqrt(Math.Pow(x - midPointX, 2) + Math.Pow(y - midPointY, 2));                    
                    result[x, y] = peakHeight * (float)Math.Pow(Math.E, -Math.Pow(distToCenterOfCurve, 2) / (2 * Math.Pow(standardDeviation, 2))) - 10f;
                }
            }
            return result;
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