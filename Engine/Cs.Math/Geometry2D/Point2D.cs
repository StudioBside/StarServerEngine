namespace Cs.Math.Geometry2D
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public readonly struct Point2D : IEquatable<Point2D>
    {
        public Point2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals([AllowNull] Point2D other)
        {
            return this.X == other.X &&
                this.Y == other.Y;
        }

        public override string ToString() => $"[Point:({this.X}, {this.Y})]";
    }
}
