using System;
using System.Windows;

namespace DrawingBase
{
    public static class VectorExtension
    {
        private static double Lerp(double firstFloat, double secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static Vector Lerp(this Vector position, Vector destination, float by)
        {
            double retX = Lerp(position.X, destination.X, by);
            double retY = Lerp(position.Y, destination.Y, by);
            return new Vector(retX, retY);
        }

        public static Vector MoveTowards(this Vector position, Vector destination, double by, double maxSpeed)
        {
            Vector differenceVector = destination - position;
            var distanceTillDestination = differenceVector.Magnitude();

            var mag = by * maxSpeed;
            if (mag > distanceTillDestination)
            {
                return new Vector(destination.X, destination.Y);
            }
            else
            {
                var movement = differenceVector.SetMagnitude(mag);
                return new Vector(position.X + movement.X, position.Y + movement.Y);
            }
        }

        public static double Magnitude(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static Vector SetMagnitude(this Vector vector, double newMagnitude)
        {
            double currentMagnitude = vector.Magnitude();
            if (newMagnitude == currentMagnitude)
            {
                return vector;
            }
            else
            {
                return new Vector(vector.X * newMagnitude / currentMagnitude, vector.Y * newMagnitude / currentMagnitude);
            }
        }
    }
}