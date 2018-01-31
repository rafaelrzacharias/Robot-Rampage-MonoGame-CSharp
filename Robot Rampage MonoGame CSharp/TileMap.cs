using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class TileMap
{
    public const int TileWidth = 32;
    public const int TileHeight = 32;
    public const int MapWidth = 50;
    public const int MapHeight = 50;

    public const int FloorTileStart = 0;
    public const int FloorTileEnd = 3;
    public const int WallTileStart = 4;
    public const int WallTileEnd = 7;
    private const int wallChance = 10;

    private static Texture2D texture;
    private static List<Rectangle> tiles = new List<Rectangle>();
    private static int[,] mapSquares = new int[MapWidth, MapHeight];
    private static Random rand = new Random();

    public static void Initialize(Texture2D tileTexture)
    {
        texture = tileTexture;
        tiles.Clear();
        tiles.Add(new Rectangle(0, 0, TileWidth, TileHeight));
        tiles.Add(new Rectangle(32, 0, TileWidth, TileHeight));
        tiles.Add(new Rectangle(64, 0, TileWidth, TileHeight));
        tiles.Add(new Rectangle(96, 0, TileWidth, TileHeight));
        tiles.Add(new Rectangle(0, 32, TileWidth, TileHeight));
        tiles.Add(new Rectangle(32, 32, TileWidth, TileHeight));
        tiles.Add(new Rectangle(64, 32, TileWidth, TileHeight));
        tiles.Add(new Rectangle(96, 32, TileWidth, TileHeight));

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                mapSquares[x, y] = FloorTileStart;
            }
        }

        GenerateRandomMap();
    }

    public static int GetSquareByPixelX(int pixelX)
    {
        return pixelX / TileWidth;
    }

    public static int GetSquareByPixelY(int pixelY)
    {
        return pixelY / TileHeight;
    }

    public static Vector2 GetSquareAtPixel(Vector2 pixelLocation)
    {
        return new Vector2(
            GetSquareByPixelX((int)pixelLocation.X), 
            GetSquareByPixelY((int)pixelLocation.Y));
    }

    public static Vector2 GetSquareCenter(int squareX, int squareY)
    {
        return new Vector2(
            (squareX * TileWidth) + (TileWidth / 2),
            (squareY * TileHeight) + (TileHeight / 2));
    }

    public static Vector2 GetSquareCenter(Vector2 square)
    {
        return GetSquareCenter((int)square.X, (int)square.Y);
    }

    public static Rectangle SquareWorldRectangle(int x, int y)
    {
        return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
    }

    public static Rectangle SquareWorldRectangle(Vector2 square)
    {
        return SquareWorldRectangle((int)square.X, (int)square.Y);
    }

    public static Rectangle SquareScreenRectangle(int x, int y)
    {
        return Camera.Transform(SquareWorldRectangle(x, y));
    }

    public static Rectangle SquareScreenRectangle(Vector2 square)
    {
        return SquareScreenRectangle((int)square.X, (int)square.Y);
    }

    public static int GetTileAtSquare(int tileX, int tileY)
    {
        if ((tileX >= 0) && (tileX < MapWidth) &&
            (tileY >= 0) && (tileY < MapHeight))
        {
            return mapSquares[tileX, tileY];
        }
        else
        {
            return -1;
        }
    }

    public static void SetTileAtSquare(int tileX, int tileY, int tile)
    {
        if ((tileX >= 0) && (tileX < MapWidth) &&
            (tileY >= 0) && (tileY < MapHeight))
        {
            mapSquares[tileX, tileY] = tile;
        }
    }

    public static int GetTileAtPixel(int pixelX, int pixelY)
    {
        return GetTileAtSquare(
            GetSquareByPixelX(pixelX), GetSquareByPixelY(pixelY));
    }

    public static int GetTileAtPixel(Vector2 pixelLocation)
    {
        return GetTileAtPixel((int)pixelLocation.X, (int)pixelLocation.Y);
    }

    public static bool IsWallTile(int tileX, int tileY)
    {
        int tileIndex = GetTileAtSquare(tileX, tileY);
        if (tileIndex == -1)
        {
            return false;
        }
        return tileIndex >= WallTileStart;
    }

    public static bool IsWallTile(Vector2 square)
    {
        return IsWallTile((int)square.X, (int)square.Y);
    }

    public static bool IsWallTileByPixel(Vector2 pixelLocation)
    {
        return IsWallTile(
            GetSquareByPixelX((int)pixelLocation.X),
            GetSquareByPixelY((int)pixelLocation.Y));
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        int startX = GetSquareByPixelX((int)Camera.Position.X);
        int endX = GetSquareByPixelX((int)Camera.Position.X + Camera.ViewportWidth);

        int startY = GetSquareByPixelY((int)Camera.Position.Y);
        int endY = GetSquareByPixelY((int)Camera.Position.Y + Camera.ViewportHeight);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                {
                    spriteBatch.Draw(texture, SquareScreenRectangle(x, y),
                        tiles[GetTileAtSquare(x, y)], Color.White);
                }
            }
        }
    }

    public static void GenerateRandomMap()
    {
        int wallChancePerSquare = wallChance;
        int floorTile = rand.Next(FloorTileStart, FloorTileEnd + 1);
        int wallTile = rand.Next(WallTileStart, WallTileEnd + 1);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                mapSquares[x, y] = floorTile;

                if ((x == 0) || (y == 0) || (x == MapWidth - 1) || (y == MapHeight - 1))
                {
                    mapSquares[x, y] = wallTile;
                    continue;
                }
                if ((x == Player.BaseSprite.WorldRectangle.X / 32) && 
                    (y == Player.BaseSprite.WorldRectangle.X / 32))
                    continue;
                if (rand.Next(0, 100) <= wallChancePerSquare)
                {
                    mapSquares[x, y] = wallTile;
                }
            }
        }
    }
}