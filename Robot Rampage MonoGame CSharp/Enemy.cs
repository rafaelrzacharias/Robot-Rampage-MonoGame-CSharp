using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class Enemy
{
    public Sprite EnemyBase;
    public Sprite EnemyClaws;
    public float EnemySpeed = 60f;
    public Vector2 CurrentTargetSquare;
    public bool Destroyed = false;
    private int collisionRadius = 24;

    public Enemy(Vector2 worldLocation, Texture2D texture, Rectangle initialFrame)
    {
        EnemyBase = new Sprite(worldLocation, texture, initialFrame, Vector2.Zero);
        EnemyBase.CollisionRadius = collisionRadius;

        Rectangle turretFrame = initialFrame;
        turretFrame.Offset(0, initialFrame.Height);
        EnemyClaws = new Sprite(worldLocation, texture, turretFrame, Vector2.Zero);
    }

    private bool ReachedTargetSquare()
    {
        return (Vector2.Distance(EnemyBase.WorldCenter, 
            TileMap.GetSquareCenter(CurrentTargetSquare)) <= 2);
    }

    private Vector2 GetNewTargetSquare()
    {
        List<Vector2> path = PathFinder.FindPath(
            TileMap.GetSquareAtPixel(EnemyBase.WorldCenter),
            TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter));

        if (path.Count > 1)
            return new Vector2(path[1].X, path[1].Y);
        else
            return TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter);
    }

    private Vector2 DetermineMoveDirection()
    {
        if (ReachedTargetSquare())
            CurrentTargetSquare = GetNewTargetSquare();

        Vector2 squareCenter = TileMap.GetSquareCenter(CurrentTargetSquare);
        return squareCenter - EnemyBase.WorldCenter;
    }

    public void Update(GameTime gameTime)
    {
        if (!Destroyed)
        {
            Vector2 direction = DetermineMoveDirection();
            direction.Normalize();

            EnemyBase.Velocity = direction * EnemySpeed;
            EnemyBase.RotateTo(direction);
            EnemyBase.Update(gameTime);

            Vector2 directionToPlayer = Player.BaseSprite.WorldCenter - 
                EnemyBase.WorldCenter;
            directionToPlayer.Normalize();

            EnemyClaws.WorldLocation = EnemyBase.WorldLocation;
            EnemyClaws.RotateTo(directionToPlayer);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!Destroyed)
        {
            EnemyBase.Draw(spriteBatch);
            EnemyClaws.Draw(spriteBatch);
        }
    }
}