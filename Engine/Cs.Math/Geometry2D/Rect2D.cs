namespace Cs.Math.Geometry2D
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public readonly struct Rect2D : IEquatable<Rect2D>
    {
        public Rect2D(Point2D position, int width, int height)
        {
            switch (GeometryConfig.RectLocalOrigin)
            {
                case Rect2dOriginType.LeftBottom:
                    this.Left = position.X;
                    this.Bottom = position.Y;
                    this.Right = this.Left + width;
                    this.Top = this.Bottom + height;
                    break;

                case Rect2dOriginType.LeftTop:
                    this.Left = position.X;
                    this.Top = position.Y;
                    this.Right = this.Left + width;
                    this.Bottom = this.Top - height;
                    break;

                default:
                    throw new Exception($"[Rect2D] unknown originType:{GeometryConfig.RectLocalOrigin}");
            }
        }

        public Rect2D(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public static Rect2D Zero => new Rect2D(0, 0, 0, 0);

        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Width => this.Right - this.Left;
        public int Height => this.Top - this.Bottom;
        public int Area => this.Width * this.Height;

        public static bool IntersectsWith(in Rect2D lhs, in Rect2D rhs)
        {
            // https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection
            // y축이 위로 향하면 등호 변경 되어야 함.
            return lhs.Left < rhs.Right &&
                lhs.Right > rhs.Left &&
                lhs.Top > rhs.Bottom &&
                lhs.Bottom < rhs.Top;
        }

        public bool Equals([AllowNull] Rect2D other)
        {
            return this.Left == other.Left &&
                this.Top == other.Top &&
                this.Right == other.Right &&
                this.Bottom == other.Bottom;
        }

        public bool Contains(in Rect2D other)
        {
            return
                this.Left <= other.Left && other.Right <= this.Right &&
                this.Bottom <= other.Bottom && other.Top <= this.Top;
        }

        public override string ToString() => $"[Rect] position:({this.Left}, {this.Bottom}) width:{this.Width} height:{this.Height}";
    }
}
