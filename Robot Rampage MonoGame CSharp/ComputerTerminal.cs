using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ComputerTerminal
{
    private Sprite activeSprite;
    private Sprite inactiveSprite;
    public Vector2 MapLocation;
    public bool Active = true;
    public float LastSpawnCounter = 0f;
    public float MinSpawnTime = 6f;

    public ComputerTerminal(Sprite activeSprite, 
        Sprite inactiveSprite, Vector2 mapLocation)
    {
        this.activeSprite = activeSprite;
        this.inactiveSprite = inactiveSprite;
        MapLocation = mapLocation;
    }

    public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
    {
        if (!Active)
            return false;

        return activeSprite.IsCircleColliding(otherCenter, otherRadius);
    }

    public void Deactivate()
    {
        Active = false;
    }

    public void Update(GameTime gameTime)
    {
        if (Active)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            LastSpawnCounter += elapsed;

            if (LastSpawnCounter > MinSpawnTime)
            {
                if (Vector2.Distance(activeSprite.WorldCenter, 
                    Player.BaseSprite.WorldCenter) > 128)
                {
                    if (EnemyManager.Enemies.Count < 
                        EnemyManager.MaxActiveEnemies)
                    {
                        EnemyManager.AddEnemy(MapLocation);
                        LastSpawnCounter = 0;
                    }
                }
            }

            activeSprite.Update(gameTime);
        }
        else
        {
            inactiveSprite.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Active)
        {
            activeSprite.Draw(spriteBatch);
        }
        else
        {
            inactiveSprite.Draw(spriteBatch);
        }
    }
}