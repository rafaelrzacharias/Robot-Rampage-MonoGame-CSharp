using Microsoft.Xna.Framework;

public static class GameManager
{
    public static int Score = 0;
    public static int CurrentWave = 0;
    public static int BaseTerminalCount = 8;
    public static int MaxTerminalCount = 15;
    public static int CurrentTerminalCount = 8;
    public static Vector2 PlayerStartLoc = new Vector2(128, 128);

    public static void StartNewWave()
    {
        CurrentWave++;

        if (CurrentTerminalCount < MaxTerminalCount)
            CurrentTerminalCount++;

        Player.BaseSprite.WorldLocation = PlayerStartLoc;
        Camera.Position = Vector2.Zero;
        WeaponManager.CurrentWeaponType = WeaponManager.WeaponType.Normal;
        WeaponManager.Shots.Clear();
        WeaponManager.PowerUps.Clear();
        EffectsManager.Effects.Clear();
        EnemyManager.Enemies.Clear();
        TileMap.GenerateRandomMap();
        GoalManager.GenerateComputers(CurrentTerminalCount);
    }

    public static void StartNewGame()
    {
        CurrentWave = 0;
        Score = 0;
        StartNewWave();
    }
}