// Vector2D class (Vector2D.cs)
using System; // Required for Math.Sqrt

namespace PicoPark
{
    public class Vector2D // Changed from partial class to regular class
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public static Vector2D Add(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D Subtract(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float LengthSquared() // Often useful for comparisons to avoid Sqrt
        {
            return X * X + Y * Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
