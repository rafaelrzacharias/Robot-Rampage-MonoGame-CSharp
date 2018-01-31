using Microsoft.Xna.Framework;

public static class Camera
{
    private static Vector2 position = Vector2.Zero;
    private static Vector2 viewportSize = Vector2.Zero;
    private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0);

    public static Vector2 Position
    {
        get { return position; }

        set {
            position = new Vector2(
                MathHelper.Clamp(value.X, worldRectangle.X, 
                worldRectangle.Width - ViewportWidth),
                MathHelper.Clamp(value.Y, worldRectangle.Y, 
                worldRectangle.Height - ViewportHeight));
        }
    }

    public static Rectangle WorldRectangle
    {
        get { return worldRectangle; }
        set { worldRectangle = value; }
    }

    public static int ViewportWidth
    {
        get { return (int)viewportSize.X; }
        set { viewportSize.X = value; }
    }

    public static int ViewportHeight
    {
        get { return (int)viewportSize.Y; }
        set { viewportSize.Y = value; }
    }

    public static Rectangle Viewport
    {
        get {
            return new Rectangle((int)Position.X, (int)Position.Y,
                ViewportWidth, ViewportHeight);
        }
    }

    public static void Move(Vector2 offset)
    {
        Position += offset;
    }

    public static bool ObjectIsVisible(Rectangle bounds)
    {
        return (Viewport.Intersects(bounds));
    }

    public static Vector2 Transform(Vector2 point)
    {
        return point - position;
    }

    public static Rectangle Transform(Rectangle rectangle)
    {
        return new Rectangle(
            rectangle.Left - (int)position.X, rectangle.Top - (int)position.Y,
            rectangle.Width, rectangle.Height);
    }
}