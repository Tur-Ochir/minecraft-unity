using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class TilePos
    {
        int xPos, yPos;

        Vector2[] uvs;

        public TilePos(int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            uvs = new Vector2[]
            {
                new Vector2(xPos / 16f + .001f, yPos / 16f + .001f),
                new Vector2(xPos / 16f + .001f, (yPos+1) / 16f - .001f),
                new Vector2((xPos+1) / 16f - .001f, (yPos+1) / 16f - .001f),
                new Vector2((xPos+1) / 16f - .001f, yPos / 16f + .001f)
            };
        }

        public Vector2[] GetUVs()
        {
            return uvs;
        }
        public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
        {
            {Tile.Ground, new TilePos(0,0)},
            {Tile.Grass, new TilePos(0,2)},
            {Tile.GrassSide, new TilePos(0,1)},
            {Tile.Dirt, new TilePos(0,0)},
            {Tile.Stone, new TilePos(1,0)},
            {Tile.Trunk, new TilePos(0,4)},
            {Tile.TrunkSide, new TilePos(0,3)},
            {Tile.Leave, new TilePos(0,5)},
        };
    }
    public enum Tile {Ground, Grass, GrassSide, Dirt, Stone, Trunk, TrunkSide, Leave}
}