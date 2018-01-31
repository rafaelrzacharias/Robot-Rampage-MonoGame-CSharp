using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class EnemyManager
{
    public static List<Enemy> Enemies = new List<Enemy>();
    public static Texture2D EnemyTexture;
    public static Rectangle EnemyInitialFrame;
    public static int MaxActiveEnemies = 30;

    public static void Initialize(Texture2D texture, Rectangle initialFrame)
    {
        EnemyTexture = texture;
        EnemyInitialFrame = initialFrame;
    }

    public static void AddEnemy(Vector2 squareLocation)
    {
        int startX = (int)squareLocation.X;
        int startY = (int)squareLocation.Y;
        Rectangle squareRect = TileMap.SquareWorldRectangle(startX, startY);

        Enemy enemy = new Enemy(new Vector2(
            squareRect.X, squareRect.Y), EnemyTexture, EnemyInitialFrame);
        enemy.CurrentTargetSquare = squareLocation;
        Enemies.Add(enemy);
    }

    public static void Update(GameTime gameTime)
    {
        for (int x = Enemies.Count - 1; x >= 0; x--)
        {
            Enemies[x].Update(gameTime);

            if (Enemies[x].Destroyed)
                Enemies.RemoveAt(x);
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }
}