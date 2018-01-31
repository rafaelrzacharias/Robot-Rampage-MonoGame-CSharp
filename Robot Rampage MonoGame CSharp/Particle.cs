using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class Particle : Sprite
{
    private Vector2 acceleration;
    private float maxSpeed;
    private int initialDuration;
    private int remainingDuration;
    private Color initialColor;
    private Color finalColor;

    public int ElapsedDuration
    {
        get { return initialDuration - remainingDuration; }
    }

    public float DurationProgress
    {
        get { return (float)ElapsedDuration / initialDuration; }
    }

    public bool IsActive
    {
        get { return (remainingDuration > 0); }
    }

    public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
        Vector2 velocity, Vector2 acceleration, float maxSpeed, int duration,
        Color initialColor, Color finalColor)
        : base(location, texture, initialFrame, velocity)
    {
        initialDuration = duration;
        remainingDuration = duration;
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
        this.initialColor = initialColor;
        this.finalColor = finalColor;
    }

    public override void Update(GameTime gameTime)
    {
        if (remainingDuration <= 0)
        {
            Expired = true;
        }
        if (!Expired)
        {
            Velocity += acceleration;
            if (Velocity.Length() > maxSpeed)
            {
                Vector2 vel = Velocity;
                vel.Normalize();
                Velocity = vel * maxSpeed;
            }

            TintColor = Color.Lerp(initialColor, finalColor, DurationProgress);
            remainingDuration--;
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            base.Draw(spriteBatch);
        }
    }
}