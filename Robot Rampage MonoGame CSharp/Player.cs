using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public static class Player
{
    public static Sprite BaseSprite;
    public static Sprite TurretSprite;

    private static Vector2 baseAngle = Vector2.Zero;
    private static Vector2 turretAngle = Vector2.Zero;
    private static float playerSpeed = 90f;

    private static Rectangle scrollArea = new Rectangle(150, 100, 500, 400);

    public static Vector2 PathingNodePosition
    {
        get { return TileMap.GetSquareAtPixel(BaseSprite.WorldCenter); }
    }

    public static void Initialize(
        Texture2D texture, Rectangle baseInitialFrame, int baseFrameCount, 
        Rectangle turretInitialFrame, int turretFrameCount, Vector2 worldLocation)
    {
        int frameWidth = baseInitialFrame.Width;
        int frameHeight = baseInitialFrame.Height;

        BaseSprite = new Sprite(
            worldLocation, texture, baseInitialFrame, Vector2.Zero);
        BaseSprite.BoundingXPadding = 4;
        BaseSprite.BoundingYPadding = 4;
        BaseSprite.AnimateWhenStopped = false;

        for (int x = 1; x < baseFrameCount; x++)
        {
            BaseSprite.AddFrame(new Rectangle(baseInitialFrame.X + (frameHeight * x), 
                baseInitialFrame.Y, frameWidth, frameHeight));
        }

        TurretSprite = new Sprite(
            worldLocation, texture, turretInitialFrame, Vector2.Zero);
        TurretSprite.Animate = false;

        for (int x = 1; x < turretFrameCount; x++)
        {
            BaseSprite.AddFrame(new Rectangle(turretInitialFrame.X + (frameHeight * x),
                turretInitialFrame.Y, frameWidth, frameHeight));
        }
    }

    private static Vector2 HandleKeyboardMovement(KeyboardState keyState)
    {
        Vector2 keyMovement = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.W))
            keyMovement.Y--;
        if (keyState.IsKeyDown(Keys.A))
            keyMovement.X--;
        if (keyState.IsKeyDown(Keys.S))
            keyMovement.Y++;
        if (keyState.IsKeyDown(Keys.D))
            keyMovement.X++;

        return keyMovement;
    }

    private static Vector2 HandleGamePadMovement(GamePadState gamePadState)
    {
        return new Vector2(
            gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y);
    }

    private static Vector2 HandleKeyboardShots(KeyboardState keyState)
    {
        Vector2 keyShots = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.NumPad1))
            keyShots = new Vector2(-1, 1);
        if (keyState.IsKeyDown(Keys.NumPad2))
            keyShots = new Vector2(0, 1);
        if (keyState.IsKeyDown(Keys.NumPad3))
            keyShots = new Vector2(1, 1);
        if (keyState.IsKeyDown(Keys.NumPad4))
            keyShots = new Vector2(-1, 0);
        if (keyState.IsKeyDown(Keys.NumPad6))
            keyShots = new Vector2(1, 0);
        if (keyState.IsKeyDown(Keys.NumPad7))
            keyShots = new Vector2(-1, -1);
        if (keyState.IsKeyDown(Keys.NumPad8))
            keyShots = new Vector2(0, -1);
        if (keyState.IsKeyDown(Keys.NumPad9))
            keyShots = new Vector2(1, -1);

        return keyShots;
    }

    private static Vector2 HandleGamePadShots(GamePadState gamePadState)
    {
        return new Vector2(
            gamePadState.ThumbSticks.Right.X, -gamePadState.ThumbSticks.Right.Y);
    }

    private static void HandleInput(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 moveAngle = Vector2.Zero;
        Vector2 fireAngle = Vector2.Zero;

        moveAngle += HandleKeyboardMovement(Keyboard.GetState());
        moveAngle += HandleGamePadMovement(GamePad.GetState(PlayerIndex.One));

        fireAngle += HandleKeyboardShots(Keyboard.GetState());
        fireAngle += HandleGamePadShots(GamePad.GetState(PlayerIndex.One));

        if (moveAngle != Vector2.Zero)
        {
            moveAngle.Normalize();
            baseAngle = moveAngle;
            moveAngle = CheckTileObstacles(elapsed, moveAngle);
        }

        if (fireAngle != Vector2.Zero)
        {
            fireAngle.Normalize();
            turretAngle = fireAngle;

            if (WeaponManager.CanFireWeapon)
            {
                WeaponManager.FireWeapon(
                    TurretSprite.WorldLocation, fireAngle * WeaponManager.WeaponSpeed);
            }
        }

        BaseSprite.RotateTo(baseAngle);
        BaseSprite.Velocity = moveAngle * playerSpeed;
        TurretSprite.RotateTo(turretAngle);

        RepositionCamera(gameTime, moveAngle);
    }

    private static void ClampToWorld()
    {
        float currentX = BaseSprite.WorldLocation.X;
        float currentY = BaseSprite.WorldLocation.Y;

        currentX = MathHelper.Clamp(
            currentX, 0, Camera.WorldRectangle.Right - BaseSprite.FrameWidth);
        currentY = MathHelper.Clamp(
            currentY, 0, Camera.WorldRectangle.Bottom - BaseSprite.FrameHeight);

        BaseSprite.WorldLocation = new Vector2(currentX, currentY);
    }

    private static void RepositionCamera(GameTime gameTime, Vector2 moveAngle)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float moveScale = playerSpeed * elapsed;

        if ((BaseSprite.ScreenRectangle.X < scrollArea.X) && (moveAngle.X < 0))
        {
            Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
        }
        if ((BaseSprite.ScreenRectangle.Right > scrollArea.Right) && (moveAngle.X > 0))
        {
            Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
        }
        if ((BaseSprite.ScreenRectangle.Y < scrollArea.Y) && (moveAngle.Y < 0))
        {
            Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
        }
        if ((BaseSprite.ScreenRectangle.Bottom > scrollArea.Bottom) && (moveAngle.Y > 0))
        {
            Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
        }
    }

    private static Vector2 CheckTileObstacles(float elapsedTime, Vector2 moveAngle)
    {
        Vector2 newHorizontalLocation = BaseSprite.WorldLocation +
            new Vector2(moveAngle.X, 0) * playerSpeed * elapsedTime;

        Vector2 newVerticalLocation = BaseSprite.WorldLocation +
            new Vector2(0, moveAngle.Y) * playerSpeed * elapsedTime;

        Rectangle newHorizontalRect = new Rectangle(
            (int)newHorizontalLocation.X, (int)BaseSprite.WorldLocation.Y,
            BaseSprite.FrameWidth, BaseSprite.FrameHeight);

        Rectangle newVerticalRect = new Rectangle(
            (int)BaseSprite.WorldLocation.X, (int)newVerticalLocation.Y,
            BaseSprite.FrameWidth, BaseSprite.FrameHeight);

        int horizLeftPixel = 0;
        int horizRightPixel = 0;

        int vertTopPixel = 0;
        int vertBottomPixel = 0;

        if (moveAngle.X < 0)
        {
            horizLeftPixel = newHorizontalRect.Left;
            horizRightPixel = BaseSprite.WorldRectangle.Left;
        }

        if (moveAngle.X > 0)
        {
            horizLeftPixel = BaseSprite.WorldRectangle.Right;
            horizRightPixel = newHorizontalRect.Right;
        }

        if (moveAngle.Y < 0)
        {
            vertTopPixel = newVerticalRect.Top;
            vertBottomPixel = BaseSprite.WorldRectangle.Top;
        }

        if (moveAngle.Y > 0)
        {
            vertTopPixel = BaseSprite.WorldRectangle.Bottom;
            vertBottomPixel = newVerticalRect.Bottom;
        }

        if (moveAngle.X != 0)
        {
            for (int x = horizLeftPixel; x < horizRightPixel; x++)
            {
                for (int y = 0; y < BaseSprite.FrameHeight; y++)
                {
                    if (TileMap.IsWallTileByPixel(
                        new Vector2(x, newHorizontalLocation.Y + y)))
                    {
                        moveAngle.X = 0;
                        break;
                    }
                }
                if (moveAngle.X == 0)
                    break;
            }
        }

        if (moveAngle.Y != 0)
        {
            for (int y = vertTopPixel; y < vertBottomPixel; y++)
            {
                for (int x = 0; x < BaseSprite.FrameWidth; x++)
                {
                    if (TileMap.IsWallTileByPixel(
                        new Vector2(newVerticalLocation.X + x, y)))
                    {
                        moveAngle.Y = 0;
                        break;
                    }
                }
                if (moveAngle.Y == 0)
                    break;
            }
        }

        return moveAngle;
    }

    public static void Update(GameTime gameTime)
    {
        HandleInput(gameTime);
        BaseSprite.Update(gameTime);
        ClampToWorld();
        TurretSprite.WorldLocation = BaseSprite.WorldLocation;
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        BaseSprite.Draw(spriteBatch);
        TurretSprite.Draw(spriteBatch);
    }
}