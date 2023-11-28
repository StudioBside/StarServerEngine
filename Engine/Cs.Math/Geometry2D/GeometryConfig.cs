namespace Cs.Math.Geometry2D
{
    public enum Rect2dOriginType
    {
        LeftTop,
        LeftBottom,
    }

    public static class GeometryConfig
    {
        public static Rect2dOriginType RectLocalOrigin { get; set; } = Rect2dOriginType.LeftBottom;
    }
}
