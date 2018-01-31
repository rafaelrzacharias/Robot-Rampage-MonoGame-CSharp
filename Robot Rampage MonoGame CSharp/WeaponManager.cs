using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class WeaponManager
{
    public static List<Particle> Shots = new List<Particle>();
    public static Texture2D Texture;
    public static Rectangle shotRectangle = new Rectangle(0, 128, 32, 32);
    public static float WeaponSpeed = 600f;

    private static float shotTimer = 0f;
    private static float shotMinTimer = 0.15f;
    private static float rocketMinTimer = 0.5f;

    public enum WeaponType { Normal, Triple, Rocket };
    public static WeaponType CurrentWeaponType = WeaponType.Normal;

    public static float WeaponTimeRemaining = 0f;
    private static float weaponTimeDefault = 30f;
    private static float tripleWeaponSplitAngle = 15f;
    private static float rocketSplashRadius = 40f;

    public static List<Sprite> PowerUps = new List<Sprite>();
    private static int maxActivePowerUps = 5;
    private static float timeSinceLastPowerUp = 0f;
    private static float timeBetweenPowerUps = 2.0f;
    private static Random rand = new Random();

    public static float WeaponFireDelay
    {
        get
        {
            if (CurrentWeaponType == WeaponType.Rocket)
            {
                return rocketMinTimer;
            }
            else
            {
                return shotMinTimer;
            }
        }
    }

    public static bool CanFireWeapon
    {
        get { return (shotTimer >= WeaponFireDelay); }
    }

    private static void AddShot(Vector2 location, Vector2 velocity, int frame)
    {
        Particle shot = new Particle(location, Texture, shotRectangle, 
            velocity, Vector2.Zero, 400f, 120, Color.White, Color.White);

        shot.AddFrame(new Rectangle(shotRectangle.X + shotRectangle.Width,
            shotRectangle.Y, shotRectangle.Width, shotRectangle.Height));

        shot.Animate = false;
        shot.Frame = frame;
        shot.RotateTo(velocity);
        Shots.Add(shot);
    }

    public static void FireWeapon(Vector2 location, Vector2 velocity)
    {
        switch (CurrentWeaponType)
        {
            case WeaponType.Normal:
                AddShot(location, velocity, 0);
                break;
            case WeaponType.Triple:
                AddShot(location, velocity, 0);

                float baseAngle = (float)Math.Atan2(velocity.Y, velocity.X);
                float offset = MathHelper.ToRadians(tripleWeaponSplitAngle);

                AddShot(location, new Vector2((float)Math.Cos(baseAngle - offset), 
                    (float)Math.Sin(baseAngle - offset)) * velocity.Length(), 0);

                AddShot(location, new Vector2((float)Math.Cos(baseAngle + offset),
                    (float)Math.Sin(baseAngle + offset)) * velocity.Length(), 0);

                break;
            case WeaponType.Rocket:
                AddShot(location, velocity, 1);
                break;
        }
                
        shotTimer = 0.0f;
    }

    private static void CheckWeaponUpgradeExpire(float elapsed)
    {
        if (CurrentWeaponType != WeaponType.Normal)
        {
            WeaponTimeRemaining -= elapsed;
            if (WeaponTimeRemaining <= 0)
            {
                CurrentWeaponType = WeaponType.Normal;
            }
        }
    }

    private static void CheckRocketSplashDamage(Vector2 location)
    {
        foreach (Enemy enemy in EnemyManager.Enemies)
        {
            if (!enemy.Destroyed)
            {
                if (enemy.EnemyBase.IsCircleColliding(
                    location, rocketSplashRadius))
                {
                    enemy.Destroyed = true;
                    GameManager.Score += 10;

                    EffectsManager.AddExplosion(
                        enemy.EnemyBase.WorldCenter, Vector2.Zero);
                }
            }
        }
    }

    private static void CheckShotEnemyImpacts(Sprite shot)
    {
        if (shot.Expired)
            return;

        foreach (Enemy enemy in EnemyManager.Enemies)
        {
            if (!enemy.Destroyed)
            {
                if (shot.IsCircleColliding(
                    enemy.EnemyBase.WorldCenter, 
                    enemy.EnemyBase.CollisionRadius))
                {
                    shot.Expired = true;
                    enemy.Destroyed = true;
                    GameManager.Score += 10;

                    if (shot.Frame == 0)
                    {
                        EffectsManager.AddExplosion(
                            enemy.EnemyBase.WorldCenter, 
                            enemy.EnemyBase.Velocity / 30);
                    }
                    else
                    {
                        if (shot.Frame == 1)
                        {
                            CreateLargeExplosion(shot.WorldCenter);
                            CheckRocketSplashDamage(shot.WorldCenter);
                        }
                    }
                }
            }
        }
    }

    private static void CheckShotWallImpacts(Sprite shot)
    {
        if (shot.Expired)
            return;

        if (TileMap.IsWallTile(TileMap.GetSquareAtPixel(shot.WorldCenter)))
        {
            shot.Expired = true;

            if (shot.Frame == 0)
            {
                EffectsManager.AddSparksEffect(shot.WorldCenter, shot.Velocity);
            }
            else
            {
                CreateLargeExplosion(shot.WorldCenter);
                CheckRocketSplashDamage(shot.WorldCenter);
            }
        }
    }

    private static void CreateLargeExplosion(Vector2 location)
    {
        EffectsManager.AddLargeExplosion(location + new Vector2(-10, -10));
        EffectsManager.AddLargeExplosion(location + new Vector2(-10, 10));
        EffectsManager.AddLargeExplosion(location + new Vector2(10, 10));
        EffectsManager.AddLargeExplosion(location + new Vector2(10, -10));
        EffectsManager.AddLargeExplosion(location);
    }

    private static void SpawnPowerUp(int x, int y, WeaponType type)
    {
        if (PowerUps.Count >= maxActivePowerUps)
            return;

        Rectangle destination = TileMap.SquareWorldRectangle(new Vector2(x, y));
        foreach (Sprite powerup in PowerUps)
        {
            if (powerup.WorldRectangle == destination)
                return;
        }

        if (PathFinder.FindPath(new Vector2(x, y), 
            Player.PathingNodePosition) != null)
        {
            Sprite newPowerup = new Sprite(
                new Vector2(destination.X, destination.Y), Texture,
                new Rectangle(64, 128, 32, 32), Vector2.Zero);
            newPowerup.Animate = false;
            newPowerup.CollisionRadius = 24;
            newPowerup.AddFrame(new Rectangle(96, 128, 32, 32));
            if (type == WeaponType.Rocket)
                newPowerup.Frame = 1;

            PowerUps.Add(newPowerup);
            timeSinceLastPowerUp = 0f;
        }
    }

    private static void CheckPowerUpSpawns(float elapsed)
    {
        timeSinceLastPowerUp += elapsed;

        if (timeSinceLastPowerUp >= timeBetweenPowerUps)
        {
            WeaponType type = WeaponType.Triple;

            if (rand.Next(0, 2) == 1)
                type = WeaponType.Rocket;

            SpawnPowerUp(rand.Next(0, TileMap.MapWidth), 
                rand.Next(0, TileMap.MapHeight), type);
        }
    }

    private static void CheckPowerUpPickups()
    {
        for (int x = PowerUps.Count - 1; x >= 0; x--)
        {
            if (Player.BaseSprite.IsCircleColliding(
                PowerUps[x].WorldCenter, PowerUps[x].CollisionRadius))
            {
                switch (PowerUps[x].Frame)
                {
                    case 0:
                        CurrentWeaponType = WeaponType.Triple;
                        break;
                    case 1:
                        CurrentWeaponType = WeaponType.Rocket;
                        break;
                }

                WeaponTimeRemaining = weaponTimeDefault;
                PowerUps.RemoveAt(x);
            }
        }
    }

    public static void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        shotTimer += elapsed;
        CheckWeaponUpgradeExpire(elapsed);

        for (int x = Shots.Count - 1; x >= 0; x--)
        {
            Shots[x].Update(gameTime);
            CheckShotWallImpacts(Shots[x]);
            CheckShotEnemyImpacts(Shots[x]);

            if (Shots[x].Expired)
            {
                Shots.RemoveAt(x);
            }
        }

        CheckPowerUpSpawns(elapsed);
        CheckPowerUpPickups();
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (Particle shot in Shots)
        {
            shot.Draw(spriteBatch);
        }

        foreach (Sprite powerup in PowerUps)
        {
            powerup.Draw(spriteBatch);
        }
    }
}